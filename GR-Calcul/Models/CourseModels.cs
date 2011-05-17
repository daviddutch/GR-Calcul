using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using System.Text;
using GR_Calcul.Misc;


namespace GR_Calcul.Models
{
    /// <summary>
    /// The Course class contains all fields necessary to describe a course
    /// </summary>
    public class Course
    {
        static private String connectionString = ConnectionManager.GetConnectionString();//System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;

        public int ID { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Clef")]
        public string Key { get; set; }

        [Required]
        [Display(Name = "Actif")]
        public bool Active { get; set; }

        [Display(Name = "Responsable")]
        public int Responsible { get; set; }


        [Required(ErrorMessage = "La date est invalide!")]
        [Display(Name = "Date de destination", Description = "dd.mm.yyyy")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [UIHint("DateFormat")]
        public DateTime DuplDestDate { get; set; }

        public string ResponsibleString { get; set; }

        public bool MyCourse { get; set; }

        //[Required]
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public string Timestamp { get; set; }

        public List<Person> Students { get; set; }


        public byte[] getByteTimestamp()
        {
            return Convert.FromBase64String(Timestamp);
        }
        public void setTimestamp(byte[] timestamp)
        {
            Timestamp = Convert.ToBase64String(timestamp);
        }

        public Course()
        {
            Students = new List<Person>();
        }

        public Course(int id_course, String name, String key, bool active, int id_person)
        {
            ID = id_course;
            Name = name;
            Key = key;
            Active = active;
            Responsible = id_person;
            Students = new List<Person>();
        }
        /// <summary>
        /// Gets all the slot ranges for the current course that have a valid date
        /// </summary>
        /// <returns>Return the slot ranges associated with the course</returns>
        public List<SlotRange> GetValidSlotRangesForCourse()
        {
            List<SlotRange> ranges = new List<SlotRange>();
            foreach (SlotRange sr in GetSlotRangesForCourse())
            {
                if (sr.locked)
                    ranges.Add(sr);
            }
            return ranges;
        }

        /// <summary>
        /// Gets all the slot ranges for the current course
        /// </summary>
        /// <returns>Return the slot ranges associated with the course</returns>
        public List<SlotRange> GetSlotRangesForCourse()
        {
            List<SlotRange> ranges = new List<SlotRange>();

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT [id_slotRange], [startRes] ,[endRes], [name] ,[id_course]" +
                                                    "FROM SlotRange SR " +
                                                    "WHERE id_course=@id " +
                                                    "ORDER BY SR.startRes ASC;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = this.ID;

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        DateTime startRes = rdr.GetDateTime(rdr.GetOrdinal("startRes"));
                        DateTime endRes = rdr.GetDateTime(rdr.GetOrdinal("endRes"));
                        string name = rdr.GetString(rdr.GetOrdinal("name"));
                        int id_course = rdr.GetInt32(rdr.GetOrdinal("id_course"));

                        SlotRange range = new SlotRange(rdr.GetInt32(rdr.GetOrdinal("id_slotRange")), startRes, endRes, name, id_course);

                        ranges.Add(range);
                    }
                    rdr.Close();

                    foreach (var range in ranges)
                    {
                        //get slots
                        cmd = new SqlCommand("SELECT [id_slot], [start], [end] FROM Slot WHERE id_slotRange=@id;", db, transaction);
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = range.id_slotRange;
                        rdr = cmd.ExecuteReader();
                        //bool hasSetDuration = false;
                        int cpt = 0;
                        while (rdr.Read())
                        {
                            int id_slot = rdr.GetInt32(rdr.GetOrdinal("id_slot"));
                            DateTime start = rdr.GetDateTime(rdr.GetOrdinal("start"));
                            DateTime end = rdr.GetDateTime(rdr.GetOrdinal("end"));

                            int startHour = start.Hour;
                            int startMinute = start.Minute;

                            int endHour = end.Hour;
                            int endMinute = end.Minute;
                            range.Startz.Add(startHour + ":" + startMinute);
                            range.Endz.Add(endHour + ":" + endMinute);
                            if (!range.Slotdate.Contains(start.Date))
                            {
                                range.Slotdate.Add(start.Date);
                            }
                            cpt++;

                            range.Slots.Add(new Slot(id_slot, start, end));
                        }
                        rdr.Close();
                        range.NumberOfSlots = cpt;

                        foreach (var slot in range.Slots)
                        {
                            //get reservations
                            cmd = new SqlCommand("SELECT [id_person], [id_slot], [numberMachines] FROM Reservation WHERE id_slot=@id;", db, transaction);
                            cmd.Parameters.Add("@id", SqlDbType.Int).Value = slot.ID;
                            rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                int id_person = rdr.GetInt32(rdr.GetOrdinal("id_person"));
                                int id_slot = rdr.GetInt32(rdr.GetOrdinal("id_slot"));
                                int numberMachines = rdr.GetInt32(rdr.GetOrdinal("numberMachines"));

                                slot.Reservations.Add(new Reservation(id_person, id_slot, numberMachines));
                            }
                            rdr.Close();
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                finally
                {
                    db.Close();
                }
            }
            catch (Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }

            return ranges;
        }

        public string GenerateAllScripts()
        {
            // get all slot ranges for this course
            List<SlotRange> slotRanges = this.GetSlotRangesForCourse();
            StringBuilder allScripts = new StringBuilder();
            
            // concat scripts
            slotRanges.ForEach(delegate(SlotRange range)
            {
                allScripts.Append(range.GenerateScript() + "###\n");
            });

            return allScripts.ToString();
        }
        /// <summary>
        /// Subscribes a user to the current course
        /// </summary>
        /// <param name="id_person">The id of the user</param>
        public void Subscribe(int? id_person)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Subscription " +
                                                   "([id_person], [id_course]) " +
                                                   "VALUES (@id_person, @id_course);", db, transaction);
                    cmd.Parameters.Add("@id_course", SqlDbType.Int).Value = ID;
                    cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = id_person;

                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }
        }
        /// <summary>
        /// Unsubscribes a user from the current course
        /// </summary>
        /// <param name="id_person">The id of the user</param>
        public void Unsubscribe(int? id_person, Course course)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.Serializable);
                try
                {

                    SqlCommand cmd = new SqlCommand("DELETE FROM Reservation " +
                        "WHERE id_person=@id_person AND id_slot IN (SELECT id_slot " +
                        "FROM slot INNER JOIN slotRange ON slot.id_slotRange=slotRange.id_slotRange WHERE id_course=@id_course);", db, transaction);

                    cmd.Parameters.Add("@id_course", SqlDbType.Int).Value = ID;
                    cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = id_person;

                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("DELETE FROM Subscription " +
                                         "WHERE id_person=@id_person AND id_course=@id_course;", db, transaction);

                    cmd.Parameters.Add("@id_course", SqlDbType.Int).Value = ID;
                    cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = id_person;

                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }
        }
    }

    /// <summary>
    /// Class containing all functions needed to interact with the course informations
    /// </summary>
    public class CourseModel
    {
        static private String connectionString = ConnectionManager.GetConnectionString();

        /// <summary>
        /// Checks if the user is subscribed to the given course
        /// </summary>
        /// <param name="userId">The id of the user to be tested</param>
        /// <param name="courseId">The id of the course to be tested</param>
        /// <returns>Returns true if the user is subscribed to the course</returns>
        public static Boolean IsUserSubscribed(int userId, int courseId)
        {
            Boolean ret = false;
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Subscription " +
                                                    "WHERE id_person=@id AND id_course=@course;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@course", SqlDbType.Int).Value = courseId;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if(rdr.Read())
                    {
                        ret = true;
                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }
            return ret;
        }
        /// <summary>
        /// Gets the whole list of the courses with information to know if the user is subscribed or not to the course
        /// </summary>
        /// <param name="id_user">The id of the user</param>
        /// <returns>Returns the course list</returns>
        public static List<Course> ListCoursesUser(int? id_user)
        {
            List<Course> list = new List<Course>();

            if (id_user == null) return list;

            list = ListActiveCourses();

            foreach (Course course in list)
            {
                if (IsUserSubscribed((int)id_user, course.ID))
                {
                    course.MyCourse = true;
                }
            }

            return list;
        }
        /// <summary>
        /// Gets the list of the courses for the given user
        /// </summary>
        /// <param name="id_user">The id of the user</param>
        /// <returns>Returns the course list of the user</returns>
        public static List<Course> ListMyCourses(int? id_user)
        {
            List<Course> list = new List<Course>();

            if (id_user == null) return list;

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT C.id_course, C.name, C.[key], C.active, R.id_person, R.firstname, R.lastname " +
                                                    "FROM Course C " +
                                                    "INNER JOIN Responsible R ON R.id_person = C.id_person " +
                                                    "INNER JOIN Subscription S ON S.id_course = C.id_course " +
                                                    "WHERE S.id_person=@id_person AND C.active=1; ",
                                                    db, transaction);

                    cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = id_user;

                    list = ListCourses(cmd);

                    foreach (Course course in list)
                    {
                        course.MyCourse = true;
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }

            return list;
        }
        /// <summary>
        /// Gets the list of the courses for the given responsible
        /// </summary>
        /// <param name="responsibleId">The id of the responsible</param>
        /// <returns>Returns the course list of the responsible</returns>
        public static List<Course> ListCourses(int? responsibleId)
        {
            List<Course> list = new List<Course>();

            if (responsibleId == null) return list;

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT C.id_course, C.name, C.[key], C.active, R.id_person, R.firstname, R.lastname " +
                                                    "FROM Course C " +
                                                    "INNER JOIN Responsible R ON R.id_person = C.id_person " +
                                                    "WHERE R.id_person=@id;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = responsibleId;

                    list = ListCourses(cmd);

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }

            return list;
        }
        /// <summary>
        /// Gets the list of all courses that are active
        /// </summary>
        /// <returns>Returns the active course list</returns>
        public static List<Course> ListActiveCourses()
        {
            List<Course> list = new List<Course>();

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT C.id_course, C.name, C.[key], C.active, R.id_person, R.firstname, R.lastname " +
                                                    "FROM Course C " +
                                                    "INNER JOIN Responsible R ON R.id_person = C.id_person " +
                                                    "WHERE C.active=1;", db, transaction);

                    list = ListCourses(cmd);

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }

            return list;
        }
        /// <summary>
        /// Gets the list of all courses
        /// </summary>
        /// <returns>Returns the whole course list</returns>
        public static List<Course> ListCourses()
        {
            List<Course> list = new List<Course>();

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT C.id_course, C.name, C.[key], C.active, R.id_person, R.firstname, R.lastname " +
                                                    "FROM Course C INNER JOIN Responsible R ON R.id_person = C.id_person; ",
                                                    db, transaction);

                    list = ListCourses(cmd);

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }

            return list;
        }
        /// <summary>
        /// Gets the course liste by executing the given Sql command
        /// </summary>
        /// <param name="cmd">The sql command to be executed</param>
        /// <returns>Returns the course list</returns>
        private static List<Course> ListCourses(SqlCommand cmd)
        {
            List<Course> list = new List<Course>();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                //string coltype = rdr.GetFieldType(rdr.GetOrdinal("active")).Name;
                string name = rdr.GetString(rdr.GetOrdinal("name"));
                string key = rdr.GetString(rdr.GetOrdinal("Key"));
                bool active = rdr.GetBoolean(rdr.GetOrdinal("active"));
                int id_person = rdr.GetInt32(rdr.GetOrdinal("id_person"));
                int id_course = rdr.GetInt32(rdr.GetOrdinal("id_course"));

                Course course = new Course(id_course, name, key,
                                            active, id_person);

                course.ResponsibleString = rdr.GetString(rdr.GetOrdinal("firstname")) + " " + rdr.GetString(rdr.GetOrdinal("lastname"));

                list.Add(course);

            }
            rdr.Close();

            return list;
        }
        /// <summary>
        /// Gets the course object for a given course id
        /// </summary>
        /// <param name="id">The id of the course</param>
        /// <returns>Returns the course</returns>
        public static Course GetCourse(int id)
        {
            Course course = null;

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT C.id_course, C.name, C.[key], C.active, R.id_person, R.firstname, R.lastname, C.timestamp " +
                                                    "FROM Course C " +
                                                    "INNER JOIN Responsible R ON R.id_person = C.id_person " +
                                                    "WHERE C.id_course=@id_course;", db, transaction);

                    cmd.Parameters.Add("@id_course", SqlDbType.Int).Value = id;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        string name = rdr.GetString(rdr.GetOrdinal("name"));
                        string key = rdr.GetString(rdr.GetOrdinal("Key"));
                        bool active = rdr.GetBoolean(rdr.GetOrdinal("active"));
                        int id_person = rdr.GetInt32(rdr.GetOrdinal("id_person"));
                        int id_course = rdr.GetInt32(rdr.GetOrdinal("id_course"));

                        course = new Course(id_course, name, key,
                                                   active, id_person);
                        course.ResponsibleString = rdr.GetString(rdr.GetOrdinal("firstname")) + " " + rdr.GetString(rdr.GetOrdinal("lastname"));
                        byte[] buffer = new byte[100];
                        rdr.GetBytes(rdr.GetOrdinal("timestamp"), 0, buffer, 0, 100);
                        course.setTimestamp(buffer);
                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch(Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }

            return course;
        }
        /// <summary>
        /// Gets the list of user subscribed to the given course
        /// </summary>
        /// <param name="id_course">The id of the course</param>
        /// <returns>Returns the person list subscribed to the course</returns>
        public static List<Person> getCourseStudents(int id_course)
        {
            List<Person> list = new List<Person>();

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT S.id_person " +
                                                    "FROM Subscription S " +
                                                    "WHERE id_course=@id_course;", db, transaction);
                    cmd.Parameters.Add("@id_course", SqlDbType.Int).Value = id_course;
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        int id_person = rdr.GetInt32(rdr.GetOrdinal("id_person"));

                        list.Add(PersonModel.getPerson(id_person, PersonType.User));
                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch(Exception e)
            {
                
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }

            return list;
        }
        /// <summary>
        /// Insert's the course in the database
        /// </summary>
        /// <param name="course">The course to be inserted</param>
        public static int CreateCourse(Course course)
        {
            int id;
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Course " +
                                   "(name, [key], active, id_person) " +
                                   "VALUES (@name, @key, @active, @id_person); " +
                                   "SELECT scope_identity()", db, transaction);

                    cmd.Parameters.Add("@name", SqlDbType.Char).Value = course.Name;
                    cmd.Parameters.Add("@key", SqlDbType.Char).Value = course.Key;
                    cmd.Parameters.Add("@active", SqlDbType.Bit).Value = course.Active;
                    cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = course.Responsible;

                    id = Convert.ToInt32(cmd.ExecuteScalar());

                    transaction.Commit();
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch(Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }
            return id;
        }
        /// <summary>
        /// Update the course in the database
        /// </summary>
        /// <param name="course">The course to be updated</param>
        public static void UpdateCourse(Course course)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {
                    byte[] timestamp = course.getByteTimestamp();

                    SqlCommand cmd = new SqlCommand("SELECT * " +
                                                    "FROM Course C " +
                                                    "WHERE C.id_course=@id_course AND C.timestamp=@timestamp;", db, transaction);

                    cmd.Parameters.Add("@id_course", SqlDbType.Int).Value = course.ID;
                    cmd.Parameters.Add("@timestamp", SqlDbType.Binary).Value = timestamp;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        rdr.Close();
                        cmd = new SqlCommand("UPDATE Course " +
                                                    "SET name=@name, [key]=@key, active=@active " +
                                                    "WHERE id_course=@id;", db, transaction);

                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = course.ID;
                        cmd.Parameters.Add("@name", SqlDbType.Char).Value = course.Name;
                        cmd.Parameters.Add("@key", SqlDbType.Char).Value = course.Key;
                        cmd.Parameters.Add("@active", SqlDbType.Bit).Value = course.Active;

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        rdr.Close();
                        transaction.Commit();
                        throw new GrException(Messages.recommencerEdit);
                    }

                    transaction.Commit();
                }
                catch(Exception e)
                {
                    if (e is GrException) throw e;
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch(Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }
        }
        /// <summary>
        /// Delete's the course in the database
        /// </summary>
        /// <param name="course">The course to delete</param>
        public static void DeleteCourse(Course course)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {
                    byte[] timestamp = course.getByteTimestamp();

                    SqlCommand cmd =new SqlCommand("SELECT * FROM Course C " +
                                                   "WHERE C.id_course=@id_course AND C.timestamp=@timestamp;", db, transaction);

                    cmd.Parameters.Add("@id_course", SqlDbType.Int).Value = course.ID;
                    cmd.Parameters.Add("@timestamp", SqlDbType.Binary).Value = timestamp;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        rdr.Close();
                        
                        cmd = new SqlCommand("DELETE FROM Course " +
                                                    "WHERE id_course=@id;", db, transaction);

                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = course.ID;

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        rdr.Close();
                        throw new GrException(Messages.recommencerEdit);
                    }

                    transaction.Commit();
                }
                catch(Exception e)
                {
                    if (e is GrException) throw e;
                    transaction.Rollback();
                    throw new GrException(e, Messages.errProd);
                }
                db.Close();
            }
            catch(Exception e)
            {
                if (e is GrException) throw e;
                throw new GrException(e, Messages.errProd);
            }
        }
    }
}