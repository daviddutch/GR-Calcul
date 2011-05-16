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
    /// <summary>
    /// Class containing Machine attributes and methods
    /// </summary>
    public class Machine
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id_machine">primary key</param>
        /// <param name="machine_name">This machine's name</param>
        /// <param name="IP">This machine's IP address</param>
        /// <param name="room">The primary key of this machine's location</param>
        public Machine(int id_machine, string machine_name, string IP, int room/*, string os*/)
        {
            // TODO: Complete member initialization
            this.id_machine = id_machine;
            this.Name = machine_name;
            this.IP = IP;
            this.id_room = room;
            //this.os = os;
        }

        /// <summary>
        /// This machine's primary key
        /// </summary>
        public int id_machine { get; set; }

        /// <summary>
        /// This machine's name
        /// </summary>
        [Required]
        [Display(Name = "Nom de la machine")]
        public string Name { get; set; }

        // RegEx for IPv4
        //[RegularExpression(@"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)",
        /// <summary>
        /// This machine's IP address
        /// </summary>
        [Display(Name = "Adresse IP de la machine")]
        public string IP { get; set; }

        /// <summary>
        /// The primary key of this machine's room
        /// </summary>
        [Required]
        [Display(Name = "Lieu")]
        public int id_room { get; set; }

        /// <summary>
        /// The name of this machine's room
        /// </summary>
        public string RoomString { get; set; }

        /// <summary>
        /// The timestamp when this machine was last edited in the DB
        /// </summary>
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public string Timestamp { get; set; }

        /// <summary>
        /// gets the timestamp
        /// </summary>
        /// <returns></returns>
        public byte[] getByteTimestamp()
        {
            return Convert.FromBase64String(Timestamp);
        }

        /// <summary>
        /// sets the timestamp
        /// </summary>
        /// <param name="timestamp"></param>
        public void setTimestamp(byte[] timestamp)
        {
            Timestamp = Convert.ToBase64String(timestamp);
        }
    }

    /// <summary>
    /// Class with machine-related methods
    /// </summary>
    public class MachineModel
    {
        static private String connectionString = ConnectionManager.GetConnectionString();//System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;

        /// <summary>
        /// Returns all the machines for a given room
        /// </summary>
        /// <param name="id_room">the primary key of the room</param>
        /// <returns>a list of Machines</returns>
        public static List<Machine> ListMachines(int id_room)
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

            return list;
        }

        /// <summary>
        /// List all machines
        /// </summary>
        /// <returns>a list with all machines</returns>
        public static List<Machine> ListMachines()
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

            return list;
        }

        /// <summary>
        /// get a machine with a given id
        /// </summary>
        /// <param name="id">the primary key for this machine</param>
        /// <returns>the Machine</returns>
        public static Machine getMachine(int id)
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

            return machine;
        }

        /// <summary>
        /// Create a new machine
        /// </summary>
        /// <param name="machine">the machine object</param>
        public static void CreateMachine(Machine machine) 
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

                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = machine.Name;
                    cmd.Parameters.Add("@IP", SqlDbType.VarChar).Value = machine.IP ?? "";
                    cmd.Parameters.Add("@id_room", SqlDbType.Int).Value = machine.id_room;

                    cmd.ExecuteNonQuery();

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
        }

        /// <summary>
        /// Update the data for a given machine
        /// </summary>
        /// <param name="machine">the machine with the new data</param>
        public static void UpdateMachine(Machine machine)
        {
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
                db.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }
        }

        /// <summary>
        /// Delete a machine
        /// </summary>
        /// <param name="machine">the machine to delete</param>
        public static void DeleteMachine(Machine machine)
        {
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

                        cmd = new SqlCommand("DELETE FROM Machine " +
                                             "WHERE id_machine=@id;", db, transaction);

                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = machine.id_machine;

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        rdr.Close();
                        Console.WriteLine("Cross modify");
                        throw new GrException(Messages.errProd);
                    }

                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    transaction.Rollback();
                    throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
                }
                db.Close();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }
        }

        /// <summary>
        /// get the names for a list of primary keys of machines
        /// </summary>
        /// <param name="list">a list of machine primary keys</param>
        /// <returns>a list of machine names</returns>
        internal List<string> getMachineNames(List<int> list)
        {
            List<string> ret = new List<string>(list.Count);
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {

                    SqlCommand cmd = new SqlCommand("SELECT name FROM Machine M " +
                        "WHERE M.id_machine in("+string.Join(", ", list)+");", db, transaction);

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        ret.Add(rdr.GetString(rdr.GetOrdinal("name")));
                    }
                    rdr.Close();
                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    System.Diagnostics.Debug.WriteLine(sqlError.Message);
                    System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                    transaction.Rollback();
                    throw new GrException(Messages.errProd);
                }
                db.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                throw (ex is GrException) ? ex : new GrException(ex, Messages.errProd);
            }
            return ret;
        }
    }
}