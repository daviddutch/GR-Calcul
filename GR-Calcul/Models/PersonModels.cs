﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;

namespace GR_Calcul.Models
{
    // CD 2011-05-02: IMPORTANT: enum names must be perfectly identical to DB Table names!
    public enum PersonType { User, Responsible, ResourceManager };

    public class Person : MembershipUser
    {
        public static readonly IDictionary<PersonType, string> pTypes = new Dictionary<PersonType, string>() 
        { 
            {PersonType.ResourceManager, "Gestionnaire des ressources"}, 
            {PersonType.Responsible, "Responsable"}, 
            {PersonType.User, "Utilisateur"}, 
        };

        public static readonly Dictionary<string, PersonType> dbTypes = new Dictionary<string, PersonType>() 
        { 
            {"RM", PersonType.ResourceManager}, 
            {"RE", PersonType.Responsible}, 
            {"US", PersonType.User}, 
        };

        public static readonly Dictionary<PersonType, string> dbTypesRev = new Dictionary<PersonType, string>() 
        { 
            {PersonType.ResourceManager, "RM"}, 
            {PersonType.Responsible, "RE"}, 
            {PersonType.User, "US"}, 
        };
        
        public static SelectList pTypeSel
        {
            get { return new SelectList(pTypes, "Key", "Value"); }
        }
        [Required]
        [Display(Name = "Type de personne")]
        public PersonType pType { get; set; }

        public int ID { get; set; }
        [Required]
        [Display(Name = "Nom")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public override string Email { get; set; }

        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        //[Required]
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public string Timestamp { get; set; }

        public byte[] getByteTimestamp()
        {
            return Convert.FromBase64String(Timestamp);
        }
        public void setTimestamp(byte[] timestamp)
        {
            Timestamp = Convert.ToBase64String(timestamp);
        }

        public Boolean IsInRole(PersonType[] roles)
        {
            foreach (PersonType r in roles)
            {
                if (r.Equals(pType))
                    return true;
            }
            return false;
        }

        public Person() { }

        public Person(PersonType type, int id_person, string firstName, string lastName, string username, string email, string password)
        {
            pType = type;
            ID = id_person;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Username = username;
        }
        public String toString()
        {
            return FirstName + " " + LastName;
        }
    }
    public class Person2
    {
        public static readonly IDictionary<PersonType, string> pTypes = new Dictionary<PersonType, string>() 
        { 
            {PersonType.ResourceManager, "Gestionnaire des ressources"}, 
            {PersonType.Responsible, "Responsable"}, 
            {PersonType.User, "Utilisateur"}, 
        };

        public static readonly Dictionary<string, PersonType> dbTypes = new Dictionary<string, PersonType>() 
        { 
            {"RM", PersonType.ResourceManager}, 
            {"RE", PersonType.Responsible}, 
            {"US", PersonType.User}, 
        };

        public static readonly Dictionary<PersonType, string> dbTypesRev = new Dictionary<PersonType, string>() 
        { 
            {PersonType.ResourceManager, "RM"}, 
            {PersonType.Responsible, "RE"}, 
            {PersonType.User, "US"}, 
        };

        public static SelectList pTypeSel
        {
            get { return new SelectList(pTypes, "Key", "Value"); }
        }

        public PersonType pType { get; set; }

        public int ID { get; set; }
        [Required]
        [Display(Name = "Nom")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        //[Required]
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public string Timestamp { get; set; }

        public byte[] getByteTimestamp()
        {
            return Convert.FromBase64String(Timestamp);
        }
        public void setTimestamp(byte[] timestamp)
        {
            Timestamp = Convert.ToBase64String(timestamp);
        }

        public Boolean IsInRole(PersonType[] roles)
        {
            foreach (PersonType r in roles)
            {
                if (r.Equals(pType))
                    return true;
            }
            return false;
        }

        public Person2() { }

        public Person2(PersonType type, int id_person, string firstName, string lastName, string username, string email, string password)
        {
            pType = type;
            ID = id_person;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Username = username;
        }
        public String toString()
        {
            return FirstName + " " + LastName;
        }
    }
    public class PersonModel
    {
        static private String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;

        public List<Person> GetResponsibles()
        {
            List<Person> list = new List<Person>();

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT RE.id_person as id_person, RE.firstname, RE.lastname, RE.email, RE.username, RE.password, RE.timestamp " +
                                                    "FROM Responsible RE " +
                                                    "ORDER BY RE.firstname;", db, transaction);


                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        //string coltype = rdr.GetFieldType(rdr.GetOrdinal("active")).Name;
                        string firstname = rdr.GetString(rdr.GetOrdinal("firstname"));
                        string lastname = rdr.GetString(rdr.GetOrdinal("lastname"));
                        string email = rdr.GetString(rdr.GetOrdinal("email"));
                        string username = rdr.GetString(rdr.GetOrdinal("username"));
                        string password = rdr.GetString(rdr.GetOrdinal("password"));
                        int id_person = rdr.GetInt32(rdr.GetOrdinal("id_person"));

                        Person person = new Person(PersonType.Responsible, id_person, firstname, lastname, username, email, password);

                        list.Add(person);

                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                }
                db.Close();
            }
            catch
            {

            }

            return list;
        }

        public List<Person> ListPerson()
        {
            List<Person> list = new List<Person>();

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * from Person ORDER BY firstname;", db, transaction);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        string firstname = rdr.GetString(rdr.GetOrdinal("firstname"));
                        string lastname = rdr.GetString(rdr.GetOrdinal("lastname"));
                        string email = rdr.GetString(rdr.GetOrdinal("email"));
                        string username = rdr.GetString(rdr.GetOrdinal("username"));
                        int id_person = rdr.GetInt32(rdr.GetOrdinal("id_person"));

                        PersonType personType = PersonType.User;
                        switch (rdr.GetString(rdr.GetOrdinal("pType")))
                        {
                            case "RM":
                                personType = PersonType.ResourceManager;
                                break;
                            case "RE":
                                personType = PersonType.Responsible;
                                break;
                            case "US":
                                personType = PersonType.User;
                                break;
                        }

                        Person person = new Person(personType, id_person, firstname, lastname, username, email, "");

                        list.Add(person);

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
        public List<Person2> ListPerson2()
        {
            List<Person2> list = new List<Person2>();

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * from Person ORDER BY firstname;", db, transaction);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        string firstname = rdr.GetString(rdr.GetOrdinal("firstname"));
                        string lastname = rdr.GetString(rdr.GetOrdinal("lastname"));
                        string email = rdr.GetString(rdr.GetOrdinal("email"));
                        string username = rdr.GetString(rdr.GetOrdinal("username"));
                        int id_person = rdr.GetInt32(rdr.GetOrdinal("id_person"));

                        PersonType personType = PersonType.User;
                        switch (rdr.GetString(rdr.GetOrdinal("pType")))
                        {
                            case "RM":
                                personType = PersonType.ResourceManager;
                                break;
                            case "RE":
                                personType = PersonType.Responsible;
                                break;
                            case "US":
                                personType = PersonType.User;
                                break;
                        }

                        Person2 person = new Person2(personType, id_person, firstname, lastname, username, email, "");

                        list.Add(person);

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

        internal void CreatePerson(Person person)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);

                try
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO [" + person.pType.ToString() + "] " +
                                                   "([email], [password], [firstname], [lastname], [username]) " +
                                                   "VALUES (@email, @password, @firstname, @lastname, @username);", db, transaction);

                    cmd.Parameters.Add("@email", SqlDbType.Char).Value = person.Email;
                    cmd.Parameters.Add("@password", SqlDbType.Char).Value = person.Password;
                    cmd.Parameters.Add("@firstname", SqlDbType.Char).Value = person.FirstName;
                    cmd.Parameters.Add("@lastname", SqlDbType.Char).Value = person.LastName;
                    cmd.Parameters.Add("@username", SqlDbType.Char).Value = person.Username;
               
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                }
                db.Close();
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
            }
        }

        internal Person getPerson(int id, PersonType pType)
        {
            Person person = null;

            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlTransaction transaction;

                conn.Open();

                transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT P.id_person as id_person, P.email, P.firstname, P.lastname, P.username, P.pType AS pType, P.timestamp " +
                            "FROM [Person] P WHERE P.id_person = @id_person AND P.pType=@pType;", conn, transaction);

                    cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@pType", SqlDbType.Char).Value = Person.dbTypesRev[pType];

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        string firstname = rdr.GetString(rdr.GetOrdinal("firstname"));
                        string lastname = rdr.GetString(rdr.GetOrdinal("lastname"));
                        string email = rdr.GetString(rdr.GetOrdinal("email"));
                        string username = rdr.GetString(rdr.GetOrdinal("username"));
                        int id_person = rdr.GetInt32(rdr.GetOrdinal("id_person"));
                        string dbType = rdr.GetString(rdr.GetOrdinal("pType"));

                        person = new Person(Person.dbTypes[dbType], id_person, firstname, lastname, username, email, "");

                        byte[] buffer = new byte[100];
                        rdr.GetBytes(rdr.GetOrdinal("timestamp"), 0, buffer, 0, 100);
                        person.setTimestamp(buffer);
                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                }
                conn.Close();
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
            }

            return person;
        }

        internal Person GetPerson(string username, string password)
        {
            Person person = null;

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT P.id_person as id_person, P.email, P.firstname, P.lastname, P.username, P.pType AS pType, P.timestamp " +
                                "FROM [Person] P WHERE P.username = @username AND P.password=@password;", db, transaction);

                    cmd.Parameters.Add("@username", SqlDbType.Char).Value = username;
                    cmd.Parameters.Add("@password", SqlDbType.Char).Value = password;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        string firstname = rdr.GetString(rdr.GetOrdinal("firstname"));
                        string lastname = rdr.GetString(rdr.GetOrdinal("lastname"));
                        string email = rdr.GetString(rdr.GetOrdinal("email"));
                        int id_person = rdr.GetInt32(rdr.GetOrdinal("id_person"));
                        string dbType = rdr.GetString(rdr.GetOrdinal("pType"));

                        person = new Person(Person.dbTypes[dbType], id_person, firstname, lastname, username, email, password);

                        byte[] buffer = new byte[100];
                        rdr.GetBytes(rdr.GetOrdinal("timestamp"), 0, buffer, 0, 100);
                        person.setTimestamp(buffer);
                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                }
                db.Close();
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
            }

            return person;
        }

        internal Person GetPerson(string username)
        {
            Person person = null;

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT P.id_person as id_person, P.email, P.firstname, P.lastname, P.username, P.pType AS pType, P.timestamp " +
                                "FROM [Person] P WHERE P.username = @username; ", db, transaction);

                    cmd.Parameters.Add("@username", SqlDbType.Char).Value = username;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        string firstname = rdr.GetString(rdr.GetOrdinal("firstname"));
                        string lastname = rdr.GetString(rdr.GetOrdinal("lastname"));
                        string email = rdr.GetString(rdr.GetOrdinal("email"));
                        int id_person = rdr.GetInt32(rdr.GetOrdinal("id_person"));
                        string dbType = rdr.GetString(rdr.GetOrdinal("pType"));

                        person = new Person(Person.dbTypes[dbType], id_person, firstname, lastname, username, email, "");

                        byte[] buffer = new byte[100];
                        rdr.GetBytes(rdr.GetOrdinal("timestamp"), 0, buffer, 0, 100);
                        person.setTimestamp(buffer);
                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                }
                db.Close();
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
            }

            return person;
        }

        internal string GetPersonByEmail(string email)
        {
            string username= null;

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT P.username " +
                            "FROM [Person] P WHERE P.email = @email;", db, transaction);

                    cmd.Parameters.Add("@email", SqlDbType.Char).Value = email;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        username = rdr.GetString(rdr.GetOrdinal("username"));
                    }
                    rdr.Close();

                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                }
                db.Close();
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
            }

            return username;
        }

        internal void UpdatePerson(Person person)
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
                    byte[] timestamp = person.getByteTimestamp();

                    SqlCommand cmd = new SqlCommand("SELECT * " +
                                                    "FROM [" + person.pType.ToString() + "] P " +
                                                    "WHERE P.id_person=@id_person AND P.timestamp=@timestamp;", db, transaction);

                    cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = person.ID;
                    cmd.Parameters.Add("@timestamp", SqlDbType.Binary).Value = timestamp;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        rdr.Close();
                        cmd = new SqlCommand("UPDATE [" + person.pType.ToString() + "] " +
                            "SET [email]=@email, [firstname]=@firstname, [lastname]=@lastname, [username]=@username " +
                                                    "WHERE id_person=@id_person", db, transaction);

                        cmd.Parameters.Add("@email", SqlDbType.Char).Value = person.Email;
                        cmd.Parameters.Add("@firstname", SqlDbType.Char).Value = person.FirstName;
                        cmd.Parameters.Add("@lastname", SqlDbType.Char).Value = person.LastName;
                        cmd.Parameters.Add("@username", SqlDbType.Char).Value = person.Username;
                        cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = person.ID;

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        rdr.Close();
                        System.Diagnostics.Debug.WriteLine("Cross modify");
                        updated = false;
                    }

                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    if (sqlError.Number == 50001)// 50001 == duplicate user. C.f. Error Message Numbers List in separate file
                    {
                        System.Diagnostics.Debug.WriteLine("here we need to inform the user of the Error (duplicate user)!!!");
                    }
                    transaction.Rollback();
                }
                db.Close();
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
            }
            if (!updated) throw new Exception("timestamp");
        }

        internal int ChangePassword(string username, string newpassword)
        {
            bool updated = true;
            Person person = GetPerson(username);
            int rows = 0;
            if (person == null)
                return rows;
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {
                    byte[] timestamp = person.getByteTimestamp();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM [" + person.pType.ToString() + "] P " +
                                "WHERE P.id_person=@id_person AND P.timestamp=@timestamp;", db, transaction);

                    cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = person.ID;
                    cmd.Parameters.Add("@timestamp", SqlDbType.Binary).Value = timestamp;

                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        rdr.Close();
                        cmd = new SqlCommand("UPDATE [" + person.pType.ToString() + "] " +
                            "SET [password]=@password WHERE id_person=@id_person; ", db, transaction);

                        cmd.Parameters.Add("@password", SqlDbType.Char).Value = newpassword;
                        cmd.Parameters.Add("@id_person", SqlDbType.Int).Value = person.ID;

                        rows = cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        rdr.Close();
                        System.Diagnostics.Debug.WriteLine("Cross modify");
                        updated = false;
                    }

                    transaction.Commit();
                    return rows;
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                }
                finally
                {
                    db.Close();
                }
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
            }
            if (!updated) throw new Exception("timestamp");
            return rows;
        }

        internal void DeletePerson(Person person)
        {
            try
            { 
                SqlConnection conn = new SqlConnection(connectionString);
                SqlTransaction transaction;

                conn.Open();

                transaction = conn.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM [" + person.pType.ToString() + "] " +
                                                    "WHERE id_person=@id;", conn, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = person.ID;

                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                }
                conn.Close();
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
            }
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    