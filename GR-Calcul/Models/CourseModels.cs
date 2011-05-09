using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using System.Text;

namespace GR_Calcul.Models
{
    public class Course
    {
        static private String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;

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

        [Required]
        [Display(Name = "Responsable")]
        public int Responsible { get; set; }

        public string ResponsibleString { get; set; }

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
                                                    "WHERE id_course=@id;", db, transaction);

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
                            //get subscriptions
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
                catch
                {
                    transaction.Rollback();
                }
                finally
                {
                    db.Close();
                }
            }
            catch
            {

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
                allScripts.Append(range.GenerateScript());
            });

            return allScripts.ToString();
        }
    }
    public class Subscription
    {
        public int id_person { get; set; }
        public int id_course { get; set; }

        public String Name { get; set; }

        public Subscription(int id_person, int id_course, String name)
        {
            this.id_person = id_person;
            this.id_course = id_course;
            this.Name = name;
        }
    }
    public class CourseModel
    {
        // CD 2011-04-21: more centralized this way for adaptation between computers/developers
        static private String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;

        public Boolean IsUserSubscribed(int userId, int courseId)
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
                catch
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch
            {

            }
            return ret;
        }
        public List<Course> ListMyCourses(int? id_user)
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
                                                    "WHERE S.id_person=@id_person; ",
                                                    db, transaction);

                    cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = id_user;

                    list = ListCourses(cmd);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch
            {

            }

            return list;
        }
        public List<Course> ListCourses(int? responsibleId)
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
                catch
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch
            {

            }

            return list;
        }
        public List<Course> ListCourses()
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
                catch
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch
            {

            }

            return list;
        }
        private List<Course> ListCourses(SqlCommand cmd)
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


        public Course GetCourse(int id)
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
                        byte[] buffer = new byte[100];
                        rdr.GetBytes(rdr.GetOrdinal("timestamp"), 0, buffer, 0, 100);
                        course.setTimestamp(buffer);
                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch
            {

            }

            return course;
        }
        public List<Person> getCourseStudents(int id_course)
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
                                                    "FROM Subscription S;", db, transaction);

                    SqlDataReader rdr = cmd.ExecuteReader();

                    PersonModel personModel = new PersonModel();

                    while (rdr.Read())
                    {
                        int id_person = rdr.GetInt32(rdr.GetOrdinal("id_person"));

                        list.Add(personModel.getPerson(id_person, PersonType.User));
                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch
            {

            }

            return list;
        }
        public void CreateCourse(Course course)
        {
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
                                   "VALUES (@name, @key, @active, @id_person);", db, transaction);
                    cmd.Parameters.Add("@name", SqlDbType.Char);
                    cmd.Parameters.Add("@key", SqlDbType.Char);
                    cmd.Parameters.Add("@active", SqlDbType.Bit);
                    cmd.Parameters.Add("@id_person", SqlDbType.Int);

                    cmd.Parameters["@name"].Value = course.Name;
                    cmd.Parameters["@key"].Value = course.Key;
                    cmd.Parameters["@active"].Value = course.Active;
                    cmd.Parameters["@id_person"].Value = course.Responsible;

                    /*
                    cmd.Parameters.Add("@name", SqlDbType.Char).Value = course.Name;
                    cmd.Parameters.Add("@key", SqlDbType.Char).Value = course.Key;
                    cmd.Parameters.Add("@active", SqlDbType.Bit).Value = course.Active;
                    cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = course.id_person;
                    */
                    /*
                    cmd.Parameters.AddWithValue("@key", "1234");
                    cmd.Parameters.AddWithValue("@active", "1");
                    cmd.Parameters.AddWithValue("@id_person", "1");
                     */
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch
            {

            }
        }

        public void UpdateCourse(Course course)
        {
            bool updated = true;

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
                                                    "SET name=@name, [key]=@key, active=@active, id_person=@id_person " +
                                                    "WHERE id_course=@id;", db, transaction);

                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = course.ID;
                        cmd.Parameters.Add("@name", SqlDbType.Char).Value = course.Name;
                        cmd.Parameters.Add("@key", SqlDbType.Char).Value = course.Key;
                        cmd.Parameters.Add("@active", SqlDbType.Bit).Value = course.Active;
                        cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = course.Responsible;

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        rdr.Close();
                        Console.WriteLine("Cross modify");
                        updated = false;
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch
            {

            }
            if (!updated) throw new Exception("timestamp");
        }

        public void DeleteCourse(int id)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Course " +
                                                    "WHERE id_course=@id;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    Console.WriteLine(sqlError);
                    transaction.Rollback();
                }
                db.Close();
            }
            catch (SqlException sqlError)
            {
                Console.WriteLine(sqlError);
            }
        }
    }
}