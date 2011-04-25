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
        public string Timestamp { get; set; }


        private int id_slotRange;

        //step1 data
        [Required]
        [Display(Name = "Nom")]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Début de Réservation")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartRes { get; set; }

        [Required]
        [Display(Name = "Fin de Réservation")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndRes { get; set; }

        [Required]
        [Display(Name = "Cours")]
        public int IdCourse { get; set; }

        public Int32[] Machines { get; set; }
        //public List<Machine> Machines { get; set; }

        //[RegularExpression(@"((2[01234]|[01]?[0123456789]):([012345]?[0123456789]))", ErrorMessage = "Donnez une heure valide.")]
        public string[] Startz { get; set; }

        //[RegularExpression(@"((2[01234]|[01]?[0123456789]):([012345]?[0123456789]))", ErrorMessage = "Donnez une heure valide.")]
        public string[] Endz { get; set; }

        public DateTime[] Slotdate { get; set; }

        //step2 data
        [Required]
        [Display(Name = "Date de départ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Beginning { get; set; }
        
        [Required]
        [Display(Name = "Durée d'un créneau")]
        //public IEnumerable<SelectListItem> SlotDuration { get; set; }
        public short SlotDuration { get; set; }

        [Required]
        [Display(Name = "Nombre de créneaux total")]
        public short NumberOfSlots { get; set; }

        //step3 data
        //public List<Slot> Slots { get; set; }

        public SlotRange()
        {
            StartRes = DateTime.Now;
            EndRes = DateTime.Now;
            Beginning = DateTime.Now;
            //Machines = new Int32[]{};
            //Slots = new List<Slot>();
            //Machines = new List<Machine>();
        }

        public SlotRange(int id_slotRange, DateTime startRes, DateTime endRes, string name, int id_course)
        {
            this.id_slotRange = id_slotRange;
            this.StartRes = startRes;
            this.EndRes = endRes;
            this.Name = name;
            this.IdCourse = id_course;
        }

        private RoomModel roomModel = new RoomModel();
        private CourseModel courseModel = new CourseModel();

        public List<Room> ListRooms()
        {
            return roomModel.ListRooms();
        }

        public List<Course> ListCourses()
        {
            return courseModel.ListCourses();
        }

        public IEnumerable<SelectListItem> ListCourses2()
        {
            List<Course> courses = ListCourses();
            List<SelectListItem> list = new List<SelectListItem>(courses.Count);
            foreach (var c in courses)
            {
                list.Add(new SelectListItem() { Value = "" + c.ID, Text = c.Name });
            }
            return list;
        }
    }

    public class Slot
    {
        public static IEnumerable<SelectListItem> durationList = new[] { 
                new SelectListItem() { Value = "1", Text = "1" },
                new SelectListItem() { Value = "2", Text = "2" }, 
                new SelectListItem() { Value = "3", Text = "3" }, 
                new SelectListItem() { Value = "4", Text = "4" }, 
                new SelectListItem() { Value = "6", Text = "6" }, 
                new SelectListItem() { Value = "8", Text = "8" }
            };

        [Required]
        [Display(Name = "Début du créneau")]
        [DataType(DataType.Date)]
        public DateTime Start { get; set; }

        [Required]
        [Display(Name = "Fin du créneau")]
        [DataType(DataType.Date)]
        public DateTime End { get; set; }
    }


    public class SlotRangeModel
    {

        static private String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;

        public SqlException CreateSlotRange(SlotRange range)
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

                    if (range.Startz.Length == range.Endz.Length && range.SlotDuration > 0 && range.Slotdate.Length == Math.Ceiling(range.Startz.Length / Math.Ceiling(24f / range.SlotDuration)))
                    {
                        int factor = 24 / range.SlotDuration;
                        //you never know...
                        if (factor != 0)
                        {
                            for (int i = 0; i < range.Startz.Length; i++)
                            {
                                string start = range.Startz[i];
                                string end = range.Endz[i];
                                DateTime dt = range.Slotdate[i / factor];

                                int startHour = Int32.Parse(start.Substring(0, start.IndexOf(':')));
                                int startMinute = Int32.Parse(start.Substring(start.IndexOf(':') + 1, start.Length - start.IndexOf(':') - 1));
                                if (startHour < 0)
                                {
                                    startHour = 0;
                                }
                                if (startHour >= 24)
                                {
                                    startHour = 23;
                                    startMinute = 59;
                                }
                                if (startMinute < 0)
                                {
                                    startMinute = 0;
                                }
                                if (startMinute > 59)
                                {
                                    startMinute = 59;
                                }

                                DateTime startDate = new DateTime(dt.Year, dt.Month, dt.Day, startHour, startMinute, 0);

                                int endHour = Int32.Parse(end.Substring(0, end.IndexOf(':')));                                
                                int endMinute = Int32.Parse(end.Substring(end.IndexOf(':') + 1, end.Length - end.IndexOf(':') - 1));
                                if (endHour < 0)
                                {
                                    endHour = 0;
                                }
                                if (endHour >= 24)
                                {
                                    endHour = 23;
                                    endMinute = 59;
                                }
                                if (endMinute < 0)
                                {
                                    endMinute = 0;
                                }
                                if (endMinute > 59)
                                {
                                    endMinute = 59;
                                }
                                DateTime endDate = new DateTime(dt.Year, dt.Month, dt.Day, endHour, endMinute, 0);


                                SqlCommand cmd2 = new SqlCommand("INSERT INTO Slot(start, [end], id_slotRange) " +
                                    "VALUES(@start, @end, @id_slotRange);", db, transaction);
                                cmd2.Parameters.Add("@start", SqlDbType.Date);
                                cmd2.Parameters.Add("@end", SqlDbType.Date);
                                cmd2.Parameters.Add("@id_slotRange", SqlDbType.Int);

                                cmd2.Parameters["@start"].Value = startDate;
                                cmd2.Parameters["@end"].Value = endDate;
                                cmd2.Parameters["@id_slotRange"].Value = rangeId;

                                cmd2.ExecuteNonQuery();
                            }
                        }
                    }

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
                    return sqlError;
                }
                finally
                {
                    db.Close();
                }
            }
            catch (SqlException sqlError)
            {
                return sqlError;
            }

            return null;
        }
    }

}