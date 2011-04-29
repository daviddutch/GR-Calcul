using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;

namespace GR_Calcul.Models
{

    public class Room
    {
        public int ID { get; private set; }

        [Required]
        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public string Timestamp { get; set; }

        public Room() { }

        public Room(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

        public byte[] getByteTimestamp()
        {
            return Convert.FromBase64String(Timestamp);
        }
        public void setTimestamp(byte[] timestamp)
        {
            Timestamp = Convert.ToBase64String(timestamp);
        }

    }



    //TODO: update, delete, create
    public class RoomModel
    {

        static private String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;

        public List<Room> ListRooms()
        {
            List<Room> list = new List<Room>();

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {

                    SqlCommand cmd = new SqlCommand("SELECT [id_room],[name] FROM [Room] R;", db, transaction);


                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {

                        int id_course = rdr.GetInt32(rdr.GetOrdinal("id_room"));
                        string name = rdr.GetString(rdr.GetOrdinal("name"));
                        Room room = new Room(id_course, name);
                        list.Add(room);

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

        public Room GetRoom(int id)
        {
            Room room = null;

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT [name], [timestamp] FROM [Room] R WHERE R.id_room=@id;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        string name = rdr.GetString(rdr.GetOrdinal("name"));
                        

                        room = new Room(id, name);
                        byte[] buffer = new byte[100];
                        rdr.GetBytes(rdr.GetOrdinal("timestamp"), 0, buffer, 0, 100);
                        room.setTimestamp(buffer);
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

            return room;
        }
    }
}
