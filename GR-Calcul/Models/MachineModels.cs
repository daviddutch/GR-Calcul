using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using GR_Calcul.Misc;

namespace GR_Calcul.Models
{   
    public class Machine
    {
        public Machine() {}

        public Machine(int id_machine, string machine_name, string IP, int room/*, string os*/)
        {
            // TODO: Complete member initialization
            this.id_machine = id_machine;
            this.Name = machine_name;
            this.IP = IP;
            this.id_room = room;
            //this.os = os;
        }

        // ID (id_machine)
        public int id_machine { get; set; }

        // name
        [Required]
        [Display(Name = "Nom de la machine")]
        public string Name { get; set; }

        // IP
        //[Required]
        [RegularExpression(@"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)",
           ErrorMessage = "Donnez une adresse IPv4 valide.")]
        [Display(Name = "Adresse IP de la machine")]
        public string IP { get; set; }

        [Required]
        [Display(Name = "Lieu")]
        public int id_room { get; set; }

        public string RoomString { get; set; }

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
    }

    public class MachineModel
    {
        static private String connectionString = ConnectionManager.GetConnectionString();//System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;

        public List<Machine> ListMachines(int id_room)
        {
            List<Machine> list = new List<Machine>();

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT M.id_machine id_machine, M.name m_name, M.IP m_ip, M.id_room id_room, R.name r_name "
                                                    //+ ", OS.name os_name "
                                                    + "FROM Machine M "
                                                    + "INNER JOIN Room R ON R.id_room = M.id_room "
                                                    //+ "INNER JOIN OS ON OS.id_os = M.id_os "
                                                    + "WHERE M.id_room=@id_room "
                                                    + ";", db, transaction);

                    cmd.Parameters.Add("@id_room", SqlDbType.Int).Value = id_room;
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        int id_machine = rdr.GetInt32(rdr.GetOrdinal("id_machine"));
                        string machine_name = rdr.GetString(rdr.GetOrdinal("m_name"));
                        string IP = rdr.GetString(rdr.GetOrdinal("m_ip"));
                        //string os = rdr.GetString(rdr.GetOrdinal("os_name"));

                        Machine machine = new Machine(id_machine, machine_name, IP,
                                                   id_room/*, os*/);

                        machine.RoomString = rdr.GetString(rdr.GetOrdinal("r_name"));

                        list.Add(machine);

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

            return list;
        }

        public List<Machine> ListMachines()
        {
            List<Machine> list = new List<Machine>();

            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlTransaction transaction;

                conn.Open();

                transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT M.id_machine id_machine, M.name m_name, M.IP m_ip, M.id_room id_room, R.name r_name "
                                                    //+ ", OS.name os_name "
                                                    + "FROM Machine M "
                                                    + "INNER JOIN Room R ON R.id_room = M.id_room "
                                                    //+ "INNER JOIN OS ON OS.id_os = M.id_os "
                                                    + ";", conn, transaction);


                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        int id_machine = rdr.GetInt32(rdr.GetOrdinal("id_machine")); 
                        string machine_name = rdr.GetString(rdr.GetOrdinal("m_name"));
                        string IP = rdr.GetString(rdr.GetOrdinal("m_ip"));
                        int id_room = rdr.GetInt32(rdr.GetOrdinal("id_room"));
                        //string os = rdr.GetString(rdr.GetOrdinal("os_name"));

                        Machine machine = new Machine(id_machine, machine_name, IP,
                                                   id_room/*, os*/);

                        machine.RoomString = rdr.GetString(rdr.GetOrdinal("r_name"));

                        list.Add(machine);

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

            return list;
        }

        public Machine getMachine(int id)
        {
            Machine machine = null;

            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlTransaction transaction;

                conn.Open();

                transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT M.id_machine id_machine, M.name m_name, M.IP m_ip, M.id_room id_room, M.timestamp, R.name r_name " +
                                                    //", OS.name os_name " +
                                                    "FROM Machine M " +
                                                    "INNER JOIN Room R ON R.id_room = M.id_room " +
                                                    // "INNER JOIN OS ON OS.id_os = M.id_os " +
                                                    "WHERE M.id_machine=@id_machine;", conn, transaction);

                    cmd.Parameters.Add("@id_machine", SqlDbType.Int).Value = id;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        int id_machine = rdr.GetInt32(rdr.GetOrdinal("id_machine"));
                        string machine_name = rdr.GetString(rdr.GetOrdinal("m_name"));
                        string IP = rdr.GetString(rdr.GetOrdinal("m_ip"));
                        int id_room = rdr.GetInt32(rdr.GetOrdinal("id_room"));
                        //string os = rdr.GetString(rdr.GetOrdinal("os_name"));

                        machine = new Machine(id_machine, machine_name, IP,
                                                   id_room/*, os*/);

                        byte[] buffer = new byte[100];
                        rdr.GetBytes(rdr.GetOrdinal("timestamp"), 0, buffer, 0, 100);
                        machine.setTimestamp(buffer);
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

            return machine;
        }

        public void CreateMachine(Machine machine)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Machine " +
                                                   "(name, IP, id_room) " +
                                                   "VALUES (@name, @IP, @id_room);", db, transaction);

                    cmd.Parameters.Add("@name", SqlDbType.Char).Value = machine.Name;
                    cmd.Parameters.Add("@IP", SqlDbType.Char).Value = machine.IP;
                    cmd.Parameters.Add("@id_room", SqlDbType.Int).Value = machine.id_room;

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


        public void UpdateMachine(Machine machine)
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
                    byte[] timestamp = machine.getByteTimestamp();

                    SqlCommand cmd = new SqlCommand("SELECT * " +
                                                    "FROM Machine M " +
                                                    "WHERE M.id_machine=@id_machine AND M.timestamp=@timestamp;", db, transaction);

                    cmd.Parameters.Add("@id_machine", SqlDbType.Int).Value = machine.id_machine;
                    cmd.Parameters.Add("@timestamp", SqlDbType.Binary).Value = timestamp;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        rdr.Close();
                        cmd = new SqlCommand("UPDATE Machine " +
                                                    "SET name=@Name, IP=@IP " +
                                                    ", id_room=@id_room " +
                                                    //", id_os=@id_os " +
                                                    "WHERE id_machine=@id_machine", db, transaction);

                        cmd.Parameters.Add("@id_machine", SqlDbType.Int).Value = machine.id_machine;
                        cmd.Parameters.Add("@Name", SqlDbType.Char).Value = machine.Name;
                        cmd.Parameters.Add("@IP", SqlDbType.Char).Value = machine.IP;
                        cmd.Parameters.Add("@id_room", SqlDbType.Int).Value = machine.id_room;

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
                db.Close();
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
            }
            if (!updated) throw new Exception("timestamp");
        }

        public void DeleteMachine(int id)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Machine " +
                                                    "WHERE id_machine=@id;", db, transaction);

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
                db.Close();
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
            }
        }
    }
}