using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebAPI.Student.Models;
using Polly;
using Polly.Retry;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace WebAPI.Student.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        static HttpClient client = new HttpClient();
        public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<JsonResult> Get()
        {
            DataTable table = new DataTable();
            var con = new NpgsqlConnection(connectionString: _configuration.GetConnectionString("StudAppConnection"));
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"Select StudentId, FullName, Class, null AS BookName from Student";
            NpgsqlDataReader reader = cmd.ExecuteReader();
            table.Load(reader);
            con.Close();
            //table.Columns.Add("bookname", typeof(string));

            var retryPolicy = Policy
                                .Handle<HttpRequestException>()
                                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            HttpClient httpClient = new HttpClient();
            JArray jsonArray = null;
            try
            {
                HttpResponseMessage response1 = await retryPolicy.ExecuteAsync(() =>
                    httpClient.GetAsync("http://webapilibrarySrv1:80/API/LIBRARY"));

                if (response1.IsSuccessStatusCode)
                {
                    // Process the successful response
                    string content = await response1.Content.ReadAsStringAsync();
                    jsonArray= JArray.Parse(content);
                   
                    Console.WriteLine("Response: " + content);
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response1.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                httpClient.Dispose();
            }
            foreach (JObject item in jsonArray)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    table.Rows[i]["bookname"] = item["bookname"];
                }
            }

           return new JsonResult(table);           
        }


        [HttpPost]
        public JsonResult Post(WebAPI.Student.Models.Student objStudent)
        {
            var con = new NpgsqlConnection(connectionString: _configuration.GetConnectionString("StudAppConnection"));
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"INSERT INTO Student (FullName, Class) VALUES (@fullName, @class)";
            cmd.Parameters.AddWithValue("fullName", objStudent.FullName);
            cmd.Parameters.AddWithValue("class", objStudent.StudentId);
            cmd.ExecuteNonQuery();
            con.Close();
            return new JsonResult("Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(WebAPI.Student.Models.Student objStudent)
        {

            var con = new NpgsqlConnection(connectionString: _configuration.GetConnectionString("StudAppConnection"));
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"UPDATE Student" +
                                $" SET FullName='{objStudent.FullName}'," +
                                $" Class='{objStudent.Class}'" +
                                $" WHERE StudentId = {objStudent.StudentId}";
            cmd.ExecuteNonQuery();
            con.Close();
            return new JsonResult("Updated Successfully");
        }


        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {

            var con = new NpgsqlConnection(connectionString: _configuration.GetConnectionString("StudAppConnection"));
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = " Delete from Student  where StudentId = " + id;
            cmd.ExecuteNonQuery();
            con.Close();
            return new JsonResult("Deleted Successfully");
        }

    }
}
