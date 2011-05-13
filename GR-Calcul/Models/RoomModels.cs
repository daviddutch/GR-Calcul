using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using GR_Calcul.Misc;

/// <summary>
/// Namespace containing all the classes related to the database
/// </summary>
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
        public int Timestamp { get; set; }

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

                    SqlCommand cmd = new SqlCommand("SELECT [id_room],[name], convert(int, [timestamp]) as timestamp FROM [Room] R;", db, transaction);


                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        int id_course = rdr.GetInt32(rdr.GetOrdinal("id_room"));
                        string name = rdr.GetString(rdr.GetOrdinal("name"));
                        Room room = new Room(id_course, name);
                        room.Timestamp = rdr.GetInt32(rdr.GetOrdinal("timestamp"));
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
                    SqlCommand cmd = new SqlCommand("SELECT [name], convert(int, [timestamp]) as timestamp FROM [Room] R WHERE R.id_room=@id;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        string name = rdr.GetString(rdr.GetOrdinal("name"));
                        
                        room = new Room(id, name);
                        room.Timestamp = rdr.GetInt32(rdr.GetOrdinal("timestamp"));
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

        public void CreateRoom(Room room)
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

        public void UpdateRoom(Room room)
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
                    SqlCommand cmd = new SqlCommand("SELECT * " +
                                                    "FROM Room R " +
                                                    "WHERE R.id_room=@id AND convert(int, R.timestamp)=@timestamp;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = room.ID;
                    cmd.Parameters.Add("@timestamp", SqlDbType.Int).Value = room.Timestamp;

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

        public void DeleteRoom(int id)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Room " +
                                                    "WHERE id_room=@id;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

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


    }
}
