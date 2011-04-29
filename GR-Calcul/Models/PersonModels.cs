﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace GR_Calcul.Models
{
    public enum PersonType { User, Responsible, ResourceManager };

    public class Person
    {
        public PersonType Type { get; set; }

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

        public Person() { }

        public Person(PersonType type, int id_person, string firstName, string lastName, string username, string email, string password)
        {
            Type = type;
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
                    SqlCommand cmd = new SqlCommand("SELECT R.id_responsible as id_person, R.firstname, R.lastname, R.email, R.username, R.password, R.timestamp " +
                                                    "FROM Responsible R " +
                                                    "ORDER BY R.firstname;", db, transaction);


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
                    SqlCommand cmd = new SqlCommand("SELECT RM.id_manager as id_person, RM.email, RM.firstname, RM.lastname, RM.username, 'RM' AS pType " +
                                                    "FROM ResourceManager RM " +
                                                    "UNION SELECT R.id_responsible, R.email, R.firstname, R.lastname, R.username, 'RE' AS pType FROM Responsible R " +
                                                    "UNION SELECT U.id_user, U.email, U.firstname, U.lastname, U.username, 'US' AS pType FROM [User] U " +
                                                    "ORDER BY RM.firstname;", db, transaction);


                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        //string coltype = rdr.GetFieldType(rdr.GetOrdinal("active")).Name;
                        string firstname = rdr.GetString(rdr.GetOrdinal("firstname"));
                        string lastname = rdr.GetString(rdr.GetOrdinal("lastname"));
                        string email = rdr.GetString(rdr.GetOrdinal("email"));
                        string username = rdr.GetString(rdr.GetOrdinal("username"));
                        //string password = rdr.GetString(rdr.GetOrdinal("password"));
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
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    