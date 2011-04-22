using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;

namespace GR_Calcul.Models
{
    public class Course
    {
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

        public Course() { }

        public Course(int id_course, String name, String key, bool active, int id_responsible)
        {
            ID = id_course;
            Name = name;
            Key = key;
            Active = active;
            Responsible = id_responsible;
        }
    }

    public class CourseModel
    {
        // CD 2011-04-21: more centralized this way for adaptation between computers/developers
        static private String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;

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
                    SqlCommand cmd = new SqlCommand("SELECT C.id_course, C.name, C.[key], C.active, R.id_responsible, R.firstname, R.lastname " +
                                                    "FROM Course C " +
                                                    "INNER JOIN Responsible R ON R.id_responsible = C.id_responsible;", db, transaction);


                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        //string coltype = rdr.GetFieldType(rdr.GetOrdinal("active")).Name;
                        string name = rdr.GetString(rdr.GetOrdinal("name"));
                        string key = rdr.GetString(rdr.GetOrdinal("Key"));
                        bool active = rdr.GetBoolean(rdr.GetOrdinal("active"));
                        int id_responsible = rdr.GetInt32(rdr.GetOrdinal("id_responsible"));
                        int id_course = rdr.GetInt32(rdr.GetOrdinal("id_course"));

                        Course course = new Course(id_course, name, key,
                                                   active, id_responsible);

                        list.Add(course);

                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch (SqlException sqlError)
            {

            }

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

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT C.id_course, C.name, C.[key], C.active, R.id_responsible, R.firstname, R.lastname " +
                                                    "FROM Course C " +
                                                    "INNER JOIN Responsible R ON R.id_responsible = C.id_responsible " +
                                                    "WHERE C.id_course=@id_course;", db, transaction);

                    cmd.Parameters.Add("@id_course", SqlDbType.Int).Value = id;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        string name = rdr.GetString(rdr.GetOrdinal("name"));
                        string key = rdr.GetString(rdr.GetOrdinal("Key"));
                        bool active = rdr.GetBoolean(rdr.GetOrdinal("active"));
                        int id_responsible = rdr.GetInt32(rdr.GetOrdinal("id_responsible"));
                        int id_course = rdr.GetInt32(rdr.GetOrdinal("id_course"));

                        course = new Course(id_course, name, key,
                                                   active, id_responsible);
                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch (SqlException sqlError)
            {

            }

            return course;
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
                                   "(name, [key], active, id_responsible) " +
                                   "VALUES (@name, @key, @active, @id_responsible);", db, transaction);
                    cmd.Parameters.Add("@name", SqlDbType.Char);
                    cmd.Parameters.Add("@key", SqlDbType.Char);
                    cmd.Parameters.Add("@active", SqlDbType.Bit);
                    cmd.Parameters.Add("@id_responsible", SqlDbType.Int);

                    cmd.Parameters["@name"].Value = course.Name;
                    cmd.Parameters["@key"].Value = course.Key;
                    cmd.Parameters["@active"].Value = course.Active;
                    cmd.Parameters["@id_responsible"].Value = course.Responsible;

                    /*
                    cmd.Parameters.Add("@name", SqlDbType.Char).Value = course.Name;
                    cmd.Parameters.Add("@key", SqlDbType.Char).Value = course.Key;
                    cmd.Parameters.Add("@active", SqlDbType.Bit).Value = course.Active;
                    cmd.Parameters.Add("@id_responsible", SqlDbType.Int).Value = course.Id_responsible;
                    */
                    /*
                    cmd.Parameters.AddWithValue("@key", "1234");
                    cmd.Parameters.AddWithValue("@active", "1");
                    cmd.Parameters.AddWithValue("@id_responsible", "1");
                     */
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch (SqlException sqlError)
            {

            }
        }

        public void UpdateCourse(Course course)
        {

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Course " +
                                                    "SET name=@name, [key]=@key, active=@active, id_responsible=@id_responsible " +
                                                    "WHERE id_course=@id;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = course.ID;
                    cmd.Parameters.Add("@name", SqlDbType.Char).Value = course.Name;
                    cmd.Parameters.Add("@key", SqlDbType.Char).Value = course.Key;
                    cmd.Parameters.Add("@active", SqlDbType.Bit).Value = course.Active;
                    cmd.Parameters.Add("@id_responsible", SqlDbType.Int).Value = course.Responsible;

                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    transaction.Rollback();
                }
                db.Close();
            }
            catch (SqlException sqlError)
            {

            }
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