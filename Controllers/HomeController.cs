using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeDataReflection.Models;
using System.Data.SqlClient;
using System.Diagnostics;

namespace RealTimeDataReflection.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<SignalServer> _context;
        public HomeController(IHubContext<SignalServer> context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }  
        public List<Student> GetAllStudents()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            var studentList = new List<Student>(); 
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlDependency.Start(connectionString);

                    string query = "select * from Student";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDependency dependency = new SqlDependency(cmd);
                    dependency.OnChange += new OnChangeEventHandler(DbChangeNotification);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var student = new Student()
                        {
                            rollno = Convert.ToInt32(reader["rollno"]),
                            fname = reader["fname"].ToString(),
                            lname = reader["lname"].ToString(),
                            city = reader["city"].ToString(),
                            mobilenum = reader["mobilenum"].ToString()
                        };
                        studentList.Add(student);
                    }
                }
                return studentList;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        private void DbChangeNotification(object sender, SqlNotificationEventArgs e)
        {
            _context.Clients.All.SendAsync("RefreshStudentData");
        }

        public bool DeleteRecord(int rollno)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            try
            {
                using(SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "Delete from Student where rollno=@rollno";
                    SqlCommand cmd = new SqlCommand(query,con);
                    cmd.Parameters.AddWithValue("@rollno",rollno);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
