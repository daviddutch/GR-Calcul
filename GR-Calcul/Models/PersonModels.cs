using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;
using GR_Calcul.Misc;
using DataAnnotationsExtensions;


namespace GR_Calcul.Models
{
    /// <summary>
    /// The 3 types of users - type names must be identical to the table names
    /// </summary>
    public enum PersonType { User, Responsible, ResourceManager };

    /// <summary>
    /// Class representing the attributes and methods of a person of type Person (used for authentication)
    /// </summary>
    public class Person : MembershipUser
    {
        /// <summary>
        /// maps the PersonTypes to user-friendly names
        /// </summary>
        public static readonly IDictionary<PersonType, string> pTypes = new Dictionary<PersonType, string>() 
        { 
            {PersonType.ResourceManager, "Gestionnaire des ressources"}, 
            {PersonType.Responsible, "Responsable"}, 
            {PersonType.User, "Utilisateur"}, 
        };

        /// <summary>
        /// maps the sql query abbreviations to PersonType enum
        /// </summary>
        public static readonly Dictionary<string, PersonType> dbTypes = new Dictionary<string, PersonType>() 
        { 
            {"RM", PersonType.ResourceManager}, 
            {"RE", PersonType.Responsible}, 
            {"US", PersonType.User}, 
        };

        /// <summary>
        /// maps the PersonType enums to the sql query abbreviations
        /// </summary>
        public static readonly Dictionary<PersonType, string> dbTypesRev = new Dictionary<PersonType, string>() 
        { 
            {PersonType.ResourceManager, "RM"}, 
            {PersonType.Responsible, "RE"}, 
            {PersonType.User, "US"}, 
        };

        /// <summary>
        /// select list for les 3 person types - used for dropdown
        /// </summary>
        public static SelectList pTypeSel
        {
            get { return new SelectList(pTypes, "Key", "Value"); }
        }

        /// <summary>
        /// the type of this person
        /// </summary>
        public PersonType pType { get; set; }

        /// <summary>
        /// the primary key of this person
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// the first name of this person
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// the last name of this person
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// the email address of this person
        /// </summary>
        public override string Email { get; set; }

        /// <summary>
        /// the user name of this person
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// the password of this person
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// the timestamp when this person was last modified in the db
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// gets the timestamp in byte[] format
        /// </summary>
        /// <returns>the timestamp in byte[] format</returns>
        public byte[] getByteTimestamp()
        {
            return Convert.FromBase64String(Timestamp);
        }

        /// <summary>
        /// set the timestamp with a byte[]
        /// </summary>
        /// <param name="timestamp">the timestamp as byte[]</param>
        public void setTimestamp(byte[] timestamp)
        {
            Timestamp = Convert.ToBase64String(timestamp);
        }

        /// <summary>
        /// whether or not this person has one of the given roles (types)
        /// </summary>
        /// <param name="roles">array of roles/types to test against</param>
        /// <returns>whether or not this person has a given role</returns>
        public Boolean IsInRole(PersonType[] roles)
        {
            foreach (PersonType r in roles)
            {
                if (r.Equals(pType))
                    return true;
            }
            return false;
        }

        ///// <summary>
        ///// empty constructor
        ///// </summary>
        //public Person() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">the type of this person</param>
        /// <param name="id_person">the primary key of this person</param>
        /// <param name="firstName">the first name of this person</param>
        /// <param name="lastName">the last name of this person</param>
        /// <param name="username">the user name of this person</param>
        /// <param name="email">the email address of this person</param>
        /// <param name="password">the password of this person</param>
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

        /// <summary>
        /// returns this person's first and last name
        /// </summary>
        /// <returns>this person's first and last name</returns>
        public String toString()
        {
            return FirstName + " " + LastName;
        }

        /// <summary>
        /// converts this person to an object of the Person2 class
        /// </summary>
        /// <returns></returns>
        public Person2 toPerson2()
        {
            return new Person2(this);
        }
    }

    /// <summary>
    /// Class representing the attributes and methods of a person of type Person2 (used for editing the person data - to avoid conflicting with the MembershipUser class)
    /// </summary>
    public class Person2
    {
        /// <summary>
        /// the type of this person
        /// </summary>
        [Required]
        [Display(Name = "Type de personne")]
        public PersonType pType { get; set; }

        /// <summary>
        /// the first name of this person
        /// </summary>
        public int ID { get; set; }
        [Required]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; }

        /// <summary>
        /// the last name of this person
        /// </summary>
        [Required]
        [Display(Name = "Nom")]
        public string LastName { get; set; }

        /// <summary>
        /// the email address of this person
        /// </summary>
        [Required]
        [Email]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// the user name of this person
        /// </summary>
        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; }

        /// <summary>
        /// the password of this person
        /// </summary>
        [Required]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        /// <summary>
        /// the timestamp when this person object was last modified in the DB
        /// </summary>
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public string Timestamp { get; set; }

        /// <summary>
        /// gets the time timestamp as byte[]
        /// </summary>
        /// <returns></returns>
        public byte[] getByteTimestamp()
        {
            return Convert.FromBase64String(Timestamp);
        }

        /// <summary>
        /// sets the timestamp using a byte[]
        /// </summary>
        /// <param name="timestamp"></param>
        public void setTimestamp(byte[] timestamp)
        {
            Timestamp = Convert.ToBase64String(timestamp);
        }

        ///// <summary>
        ///// whether this person is in one of the given roles/types
        ///// </summary>
        ///// <param name="roles">list of person types</param>
        ///// <returns>whether this person is in one of the given roles/types</returns>
        //public Boolean IsInRole(PersonType[] roles)
        //{
        //    foreach (PersonType r in roles)
        //    {
        //        if (r.Equals(pType))
        //            return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Person2() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="person">person object of "Person" type</param>
        public Person2(Person person) {
            pType = person.pType;
            ID = person.ID;
            FirstName = person.FirstName;
            LastName = person.LastName;
            Email = person.Email;
            Password = person.Password;
            Username = person.Username;
            Timestamp = person.Timestamp;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">the type of this person</param>
        /// <param name="id_person">the primary key of this person</param>
        /// <param name="firstName">the first name of this person</param>
        /// <param name="lastName">the last name of this person</param>
        /// <param name="username">the username of this person</param>
        /// <param name="email">the email address of this person</param>
        /// <param name="password">the password of this person</param>
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

        /// <summary>
        /// the fist and last name of this person
        /// </summary>
        /// <returns>the fist and last name of this person</returns>
        public String toString()
        {
            return FirstName + " " + LastName;
        }
    }

    /// <summary>
    /// Class containing various methods related to Person and Person2
    /// </summary>
    public class PersonModel
    {
        //static private String connectionString = ConnectionManager.GetConnectionString();//System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;

        /// <summary>
        /// Converts a list of persons of type Person to a list of persons of class Person2
        /// </summary>
        /// <param name="list">list of persons of vlyss Person</param>
        /// <returns>list of persons of type Person2</returns>
        public static List<Person2> ConvertPersons(List<Person> list)
        {
            List<Person2> result = new List<Person2>(list.Count);
            foreach (var p in list)
            {
                result.Add(new Person2(p));
            }
            return result;
        }

        /// <summary>
        /// get a list of all persons of type "Responsible"
        /// </summary>
        /// <returns>List of persons of class Person</returns>
        public static List<Person> GetResponsibles()
        {
            List<Person> list = new List<Person>();

            try
            {
                SqlConnection db = new SqlConnection(ConnectionManager.GetConnectionString());
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT RE.id_person as id_person, RE.firstname, RE.lastname, RE.email, RE.username, RE.password, RE.timestamp " +
                                                    "FROM Responsible RE " +
                                                    "ORDER BY RE.lastname;", db, transaction);


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
                    throw new GrException(sqlError, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception ex)
            {
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }

            return list;
        }

        /// <summary>
        /// Gets the list of all the ResourceManagers
        /// </summary>
        /// <returns>list of ResourceManagers</returns>
        public static List<Person> GetResourceManagers()
        {
            List<Person> list = new List<Person>();

            try
            {
                SqlConnection db = new SqlConnection(ConnectionManager.GetConnectionString());
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT RM.id_person as id_person, RM.firstname, RM.lastname, RM.email, RM.username, RM.password, RM.timestamp " +
                                                    "FROM ResourceManager RM " +
                                                    "ORDER BY RM.lastname;", db, transaction);


                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
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
                    throw new GrException(sqlError, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }

            return list;
        }

        /// <summary>
        /// Gets the list of all Persons
        /// </summary>
        /// <returns>the list of all Persons</returns>
        public static List<Person> ListPerson()
        {
            List<Person> list = new List<Person>();

            try
            {
                SqlConnection db = new SqlConnection(ConnectionManager.GetUnauthentifiedConnectionString());
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
                catch (SqlException sqlException)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine(sqlException.Message);
                    System.Diagnostics.Debug.WriteLine(sqlException.StackTrace);
                    throw new GrException(sqlException, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }

            return list;
        }

        /// <summary>
        /// Creates a person
        /// </summary>
        /// <param name="person">the person to create</param>
        public static void CreatePerson(Person2 person)
        {
            try
            {
                SqlConnection db = new SqlConnection(ConnectionManager.GetConnectionString());
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
                    throw new GrException(sqlError, (sqlError.Number > 50000) ? sqlError.Message : Messages.uniqueUserEmail);
                }
                finally
                {
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }
        }

        /// <summary>
        /// gets a person object
        /// </summary>
        /// <param name="id">the primary key of the person</param>
        /// <param name="pType">the type of the person</param>
        /// <returns>a Person</returns>
        public static Person getPerson(int id, PersonType pType)
        {
            Person person = null;

            try
            {
                SqlConnection conn = new SqlConnection(ConnectionManager.GetUnauthentifiedConnectionString());
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
                    throw new GrException(sqlError, Messages.errProd);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }

            return person;
        }

        /// <summary>
        /// Gets a person for a given username and password
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="password">the password</param>
        /// <returns>the person with the given username and password</returns>
        public static Person GetPerson(string username, string password)
        {
            Person person = null;

            try
            {
                SqlConnection db = new SqlConnection(ConnectionManager.GetUnauthentifiedConnectionString());
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
                    throw new GrException(sqlError, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }

            return person;
        }

        /// <summary>
        /// Get a person with a given username
        /// </summary>
        /// <param name="username">the person's username</param>
        /// <returns>the person</returns>
        public static Person GetPerson(string username)
        {
            Person person = null;

            try
            {
                SqlConnection db = new SqlConnection(ConnectionManager.GetUnauthentifiedConnectionString());
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
                    throw new GrException(sqlError, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }

            return person;
        }

        /// <summary>
        /// gets a person for a given email address
        /// </summary>
        /// <param name="email">email address</param>
        /// <returns>the person</returns>
        public static string GetPersonByEmail(string email)
        {
            string username= null;

            try
            {
                SqlConnection db = new SqlConnection(ConnectionManager.GetUnauthentifiedConnectionString());
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
                    throw new GrException(sqlError, Messages.errProd);
                }
                db.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }

            return username;
        }

        /// <summary>
        /// update a person
        /// </summary>
        /// <param name="person">the person object containing the new data to update</param>
        public static void UpdatePerson(Person2 person)
        {
            try
            {
                SqlConnection db = new SqlConnection(ConnectionManager.GetConnectionString());
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
                        throw new GrException(Messages.recommencerEdit);
                    }

                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                    throw new GrException(sqlError, (sqlError.Number > 50000) ? sqlError.Message : Messages.uniqueUserEmail);
                }
                finally
                {
                    db.Close();
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }
        }

        /// <summary>
        /// changes the password for a given person
        /// </summary>
        /// <param name="username">the username of the person</param>
        /// <param name="newpassword">the new password of the person</param>
        /// <returns>the number of changed rows in the db (should be 1)</returns>
        public static int ChangePassword(string username, string newpassword)
        {
            Person person = GetPerson(username);
            int rows = 0;
            if (person == null)
                return rows;
            try
            {
                SqlConnection db = new SqlConnection(ConnectionManager.GetConnectionString());
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
                        throw new GrException(Messages.recommencerEdit);
                    }

                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                    throw new GrException(sqlError, Messages.errProd);
                }
                finally
                {
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }
            return rows;
        }

        /// <summary>
        /// delete a person from the db
        /// </summary>
        /// <param name="person">the person to delete</param>
        public static void DeletePerson(Person2 person)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionManager.GetConnectionString());
                SqlTransaction transaction;

                conn.Open();

                transaction = conn.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {
                    byte[] timestamp = person.getByteTimestamp();

                    SqlCommand cmd = new SqlCommand("SELECT * " +
                                                    "FROM [" + person.pType.ToString() + "] " +
                                                    "WHERE id_person=@id AND timestamp=@timestamp;", conn, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = person.ID;
                    cmd.Parameters.Add("@timestamp", SqlDbType.Binary).Value = timestamp;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        rdr.Close();

                        cmd = new SqlCommand("DELETE FROM [" + person.pType.ToString() + "] " +
                                             "WHERE id_person=@id;", conn, transaction);

                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = person.ID;

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        rdr.Close();
                        Console.WriteLine("Cross modify");
                        throw new GrException(Messages.recommencerDelete);
                    }

                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                    throw new GrException(sqlError, Messages.errProd);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }
        }

        /// <summary>
        /// Get a csv list of email addresses for a list of persons
        /// </summary>
        /// <param name="persons">the list of persons</param>
        /// <returns>the csv list of email addresses</returns>
        public static String GetEmailCSV(List<Person> persons)
        {
            List<string> emails = new List<string>();
            persons.ForEach(delegate(Person person)
            {
                emails.Add(person.Email);
            });

            return string.Join(", ", emails);
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    