using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using GR_Calcul.Misc;


namespace GR_Calcul.Models
{

    public class Room
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Nom")]
        [StringLength(20)]
        public string Name { get; set; }

        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public string Timestamp { get; set; }

        //public string os { get; set; }

        public byte[] getByteTimestamp()
        {
            return Convert.FromBase64String(Timestamp);
        }
        public void setTimestamp(byte[] timestamp)
        {
            Timestamp = Convert.ToBase64String(timestamp);
        }

        public Room() { }

        public Room(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

    }

    public class RoomModel
    {

        static private String connectionString = ConnectionManager.GetConnectionString();

        public static List<Room> ListRooms()
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

                    SqlCommand cmd = new SqlCommand("SELECT [id_room],[name], [timestamp] FROM [Room] R;", db, transaction);


                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        int id_course = rdr.GetInt32(rdr.GetOrdinal("id_room"));
                        string name = rdr.GetString(rdr.GetOrdinal("name"));
                        Room room = new Room(id_course, name);

                        byte[] buffer = new byte[100];
                        rdr.GetBytes(rdr.GetOrdinal("timestamp"), 0, buffer, 0, 100);
                        room.setTimestamp(buffer);

                        list.Add(room);

                    }
                    rdr.Close();
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

            return list;
        }

        public static Room GetRoom(int id)
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
                finally
                {
                    db.Close();
                }
            }
            catch
            {

            }

            return room;
        }

        public static void CreateRoom(Room room)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Room " +
                                                   "(name) " +
                                                   "VALUES (@name);", db, transaction);

                    cmd.Parameters.Add("@name", SqlDbType.Char).Value = room.Name;

                    cmd.ExecuteNonQuery();

                    transaction.Commit();
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
        }

        public static void UpdateRoom(Room room)
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

                    byte[] timestamp = room.getByteTimestamp();

                    SqlCommand cmd = new SqlCommand("SELECT * " +
                                                    "FROM Room R " +
                                                    "WHERE R.id_room=@id AND R.timestamp=@timestamp;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = room.ID;
                    cmd.Parameters.Add("@timestamp", SqlDbType.Binary).Value = timestamp;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        rdr.Close();
                        cmd = new SqlCommand("UPDATE Room " +
                            "SET name=@Name WHERE id_room=@id;", db, transaction);

                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = room.ID;
                        cmd.Parameters.Add("@Name", SqlDbType.Char).Value = room.Name;

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
        }

        public static void DeleteRoom(int id, Room room)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {

                    byte[] timestamp = room.getByteTimestamp();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM Room R " +
                                                   "WHERE R.id_room=@id_room AND C.timestamp=@timestamp;", db, transaction);

                    cmd.Parameters.Add("@id_room", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@timestamp", SqlDbType.Binary).Value = timestamp;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        rdr.Close();

                        cmd = new SqlCommand("DELETE FROM Room " +
                                                    "WHERE id_room=@id;", db, transaction);

                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        rdr.Close();
                        errMsg += " " + Messages.recommencerDelete;
                        Console.WriteLine("Cross modify");
                    }


                    transaction.Commit();
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
        }


    }
}
