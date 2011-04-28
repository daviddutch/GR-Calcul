using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;

namespace GR_Calcul.Models
{

    public class SlotRange
    {

        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public int Timestamp { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int id_slotRange { get; set; }

        //step1 data
        [Required]
        [Display(Name = "Nom")]
        [StringLength(20)]
        public string Name { get; set; }

        [Required(ErrorMessage="La date est invalide!")]
        [Display(Name = "Début de Réservation", Description="dd/mm/yyyy")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [UIHint("lollipop")]
        public DateTime StartRes { get; set; }

        [Required(ErrorMessage = "La date est invalide!")]
        [Display(Name = "Fin de Réservation", Description = "dd/mm/yyyy")]
        [DataType(DataType.Date)]
        [UIHint("lollipop")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndRes { get; set; }

        [Required]
        [Display(Name = "Cours")]
        public int IdCourse { get; set; }

        //[RegularExpression(@"((2[01234]|[01]?[0123456789]):([012345]?[0123456789]))", ErrorMessage = "Donnez une heure valide.")]
        
        public List<string> Startz { get; set; }

        public List<string> Endz { get; set; }

        public List<Int32> Machines { get; set; }

        public List<DateTime> Slotdate { get; set; }

        public List<string> StartzAdded { get; set; }
        public List<string> EndzAdded { get; set; }
        public List<DateTime> SlotdateAdded { get; set; }

        public List<Slot> Slots { get; set; }


        //step2 data
        [Required]
        [Display(Name = "Date de départ", Description = "dd/mm/yyyy")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [UIHint("lollipop")]
        public DateTime Beginning { get; set; }
        
        [Required]
        [Display(Name = "Durée d'un créneau")]
        public int SlotDuration { get; set; }

        [Required]
        [Display(Name = "Nombre de créneaux total")]
        public int NumberOfSlots { get; set; }


        public SlotRange()
        {
            StartRes = DateTime.Now;
            EndRes = DateTime.Now;
            Beginning = DateTime.Now;

            Slots = new List<Slot>();
            Machines = new List<Int32>();
            Startz = new List<string>();
            Endz = new List<string>();
            Slotdate = new List<DateTime>();
            StartzAdded = new List<string>();
            EndzAdded = new List<string>();
            SlotdateAdded = new List<DateTime>();
        }

        public SlotRange(int id_slotRange, DateTime startRes, DateTime endRes, string name, int id_course) : this()
        {
            this.id_slotRange = id_slotRange;
            this.StartRes = startRes;
            this.EndRes = endRes;
            this.Name = name;
            this.IdCourse = id_course;
        }

        public List<Slot> GetSlotsForDate(DateTime date)
        {
            List<Slot> slots = new List<Slot>(Slots.Count);
            foreach (var slot in Slots)
            {
                int day = slot.Start.Day;
                int month = slot.Start.Month;
                int year = slot.Start.Year;
                if (date.Day == day && date.Month == month && date.Year == year)
                {
                    slots.Add(slot);
                }
            }

            return slots;
        }

        public int getNumberOfSlotCols()
        {
            int max = 0;
            foreach (var date in Slotdate)
            {
                int p = GetSlotsForDate(date).Count;
                if (p > max)
                    max = p;
            }
            return max;
        }

    }

    public class Slot
    {
        public static List<SelectListItem> durationList;

        public static IEnumerable<int> durations = new[] { 1,2,3,4,6,8 };

        static Slot()
        {
            durationList = new List<SelectListItem>(durations.Count());
            foreach (var i in durations)
            {
                durationList.Add(new SelectListItem() { Value = ""+i, Text = ""+i });
            }
        }

        public Slot(DateTime start, DateTime end){
            Start = start;
            End = end;
        }

        [Required]
        [Display(Name = "Début du créneau", Description = "dd/mm/yyyy")]
        [DataType(DataType.DateTime)]
        [UIHint("lollipop")]
        public DateTime Start { get; set; }

        [Required]
        [Display(Name = "Fin du créneau", Description = "dd/mm/yyyy")]
        [DataType(DataType.DateTime)]
        [UIHint("lollipop")]
        public DateTime End { get; set; }
    }


    public class SlotRangeModel
    {

        static private String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;

        public SlotRange GetSlotRange(int id)
        {
            SlotRange range = null;

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT [startRes] ,[endRes] ,"  +
                        "[name] ,[id_course] , convert(int, [timestamp]) as timestamp FROM SlotRange WHERE id_slotRange=@id;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    SqlDataReader rdr = cmd.ExecuteReader();
                    bool hasFound = false;
                    if (rdr.Read())
                    {
                        DateTime startRes = rdr.GetDateTime(rdr.GetOrdinal("startRes"));
                        DateTime endRes = rdr.GetDateTime(rdr.GetOrdinal("endRes"));
                        string name = rdr.GetString(rdr.GetOrdinal("name"));
                        int id_course = rdr.GetInt32(rdr.GetOrdinal("id_course"));

                        range = new SlotRange(id, startRes, endRes, name, id_course);
                        range.Timestamp = rdr.GetInt32(rdr.GetOrdinal("timestamp"));
                        hasFound = true;
                    }
                    rdr.Close();
                    if (!hasFound)
                    {
                        return null;
                    }

                    //get machines
                    SqlCommand cmd2 = new SqlCommand("SELECT [id_machine] FROM MachineSlotRange WHERE id_slotRange=@id;", db, transaction);

                    cmd2.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    SqlDataReader rdr2 = cmd2.ExecuteReader();

                    while (rdr2.Read())
                    {
                        int id_machine = rdr2.GetInt32(rdr2.GetOrdinal("id_machine"));
                        range.Machines.Add(id_machine);
                    }
                    rdr2.Close();

                    //get slots
                    cmd2 = new SqlCommand("SELECT [start], [end] FROM Slot WHERE id_slotRange=@id;", db, transaction);
                    cmd2.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    rdr2 = cmd2.ExecuteReader();
                    bool hasSetDuration = false;
                    int cpt = 0;
                    while (rdr2.Read())
                    {
                        DateTime start = rdr2.GetDateTime(rdr2.GetOrdinal("start"));
                        DateTime end = rdr2.GetDateTime(rdr2.GetOrdinal("end"));

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

                        range.Slots.Add(new Slot(start, end));
                    }
                    rdr2.Close();
                    range.NumberOfSlots = cpt;

                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    transaction.Rollback();
                }
                finally
                {
                    db.Close();
                }
            }
            catch (SqlException sqlError)
            {

            }

            return range;
        }

        public void CreateSlotRange(SlotRange range)
        {
            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;
                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_getIdentitySlotRange", db, transaction);
                    SqlParameter sp = new SqlParameter("@id", SqlDbType.Int);
                    sp.Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(sp);

                    cmd.Parameters.Add("@startRes", SqlDbType.Date);
                    cmd.Parameters.Add("@endRes", SqlDbType.Date);
                    cmd.Parameters.Add("@name", SqlDbType.Char);
                    cmd.Parameters.Add("@id_course", SqlDbType.Int);

                    cmd.Parameters["@startRes"].Value = range.StartRes;
                    cmd.Parameters["@endRes"].Value = range.EndRes;
                    cmd.Parameters["@name"].Value = range.Name;
                    cmd.Parameters["@id_course"].Value = range.IdCourse;

                    cmd.ExecuteNonQuery();

                    int rangeId = Int32.Parse(sp.Value.ToString());
                    InsertAllSlots(range, rangeId, db, transaction);

                    foreach (var machine in range.Machines)
                    {
                        SqlCommand cmd3 = new SqlCommand("INSERT INTO MachineSlotRange(id_machine, id_slotRange) " +
                        "VALUES(@id_machine, @id_slotRange);", db, transaction);

                        cmd3.Parameters.Add("@id_machine", SqlDbType.Int);
                        cmd3.Parameters.Add("@id_slotRange", SqlDbType.Int);

                        cmd3.Parameters["@id_machine"].Value = machine;
                        cmd3.Parameters["@id_slotRange"].Value = rangeId;

                        cmd3.ExecuteNonQuery();
                    }


                    transaction.Commit();
                }
                catch (SqlException sqlError)
                {
                    transaction.Rollback();                    
                }
                finally
                {
                    db.Close();
                }
            }
            catch (SqlException sqlError)
            {
            }

        }

        private void InsertAllSlots(SlotRange range, int rangeId, SqlConnection db, SqlTransaction transaction) 
        { 
			InsertSlots(range.Startz, range.Endz, range.Slotdate, range.SlotDuration, rangeId, db, transaction);
			InsertSlots(range.StartzAdded, range.EndzAdded, range.SlotdateAdded, range.SlotDuration, rangeId, db, transaction);
		}

        private void InsertSlots(List<string> start, List<string> end, List<DateTime> dt, int slotDuration, int rangeId, SqlConnection db, SqlTransaction transaction)
        {
            if (start.Count == end.Count && dt.Count > 0)
            {
                int factor = (int)Math.Ceiling((float)start.Count / dt.Count);
                for (int i = 0; i < start.Count; i++)
                {
                    string st = start[i];
                    string ed = end[i];
                    DateTime d = dt[i / factor];

                    InsertSlot(st, ed, d, rangeId, db, transaction);
                }
            }
        }

        private void InsertSlot(string start, string end, DateTime dt, int rangeId, SqlConnection db, SqlTransaction transaction)
        {
            DateTime startDate = buildDateTime(start, dt);

            DateTime endDate = buildDateTime(end, dt);

            SqlCommand cmd2 = new SqlCommand("INSERT INTO Slot(start, [end], id_slotRange) " +
                "VALUES(@start, @end, @id_slotRange);", db, transaction);
            cmd2.Parameters.Add("@start", SqlDbType.DateTime);
            cmd2.Parameters.Add("@end", SqlDbType.DateTime);
            cmd2.Parameters.Add("@id_slotRange", SqlDbType.Int);

            cmd2.Parameters["@start"].Value = startDate;
            cmd2.Parameters["@end"].Value = endDate;
            cmd2.Parameters["@id_slotRange"].Value = rangeId;

            cmd2.ExecuteNonQuery();
        }

        private DateTime buildDateTime(string time, DateTime datePart){
            int hour = Int32.Parse(time.Substring(0, time.IndexOf(':')));
            int minute = Int32.Parse(time.Substring(time.IndexOf(':') + 1, time.Length - time.IndexOf(':') - 1));
            if (hour < 0)
            {
                hour = 0;
            }
            if (hour >= 24)
            {
                hour = 23;
                minute = 59;
            }
            if (minute < 0)
            {
                minute = 0;
            }
            if (minute > 59)
            {
                minute = 59;
            }

            return new DateTime(datePart.Year, datePart.Month, datePart.Day, hour, minute, 0);
        }

        public void UpdateSlotRange(SlotRange range)
        {
            bool updated = false;

            try
            {
                SqlConnection db = new SqlConnection(connectionString);
                SqlTransaction transaction;

                db.Open();

                transaction = db.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {
                    int timestamp = range.Timestamp;

                    SqlCommand cmd = new SqlCommand("SELECT * FROM SlotRange R " +
                            "WHERE R.[id_slotRange]=@id AND convert(int, R.timestamp)=@timestamp;", db, transaction);

                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = range.id_slotRange;
                    cmd.Parameters.Add("@timestamp", SqlDbType.Int).Value = timestamp;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        rdr.Close();
                        cmd = new SqlCommand("UPDATE SlotRange SET startRes=@startRes, endRes=@endRes, " +
                            "name=@name, id_course=@idCourse WHERE id_slotRange=@id;", db, transaction);
                        cmd.Parameters.Add("@startRes", SqlDbType.DateTime).Value = range.StartRes;
                        cmd.Parameters.Add("@endRes", SqlDbType.DateTime).Value = range.EndRes;
                        cmd.Parameters.Add("@name", SqlDbType.Char).Value = range.Name;
                        cmd.Parameters.Add("@idCourse", SqlDbType.Int).Value = range.IdCourse;
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = range.id_slotRange;
                        cmd.ExecuteNonQuery();
                        updated = true;
                        SqlCommand cmd2 = new SqlCommand("DELETE FROM Slot WHERE id_slotRange=@id;", db, transaction);
                        cmd2.Parameters.Add("@id", SqlDbType.Int).Value = range.id_slotRange;
                        cmd2.ExecuteNonQuery();

                        InsertAllSlots(range, range.id_slotRange, db, transaction);

                        transaction.Commit();
                    }
                    else
                    {
                        rdr.Close();
                        Console.WriteLine("Cross modify");
                    }
                }
                catch (SqlException sqlError)
                {
                    transaction.Rollback();
                }
                finally
                {
                    db.Close();
                }
            }
            catch (SqlException sqlError)
            {

            }
            if (!updated) throw new Exception("timestamp");
        }
    }

}