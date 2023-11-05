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
        public JsonResult Get()
        {
            DataTable table = new DataTable();
            var con = new NpgsqlConnection(connectionString: _configuration.GetConnectionString("StudAppConnection"));
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"Select StudentId, FullName, Class from Student";
            NpgsqlDataReader reader = cmd.ExecuteReader();
            table.Load(reader);
            con.Close();

            var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));


            HttpResponseMessage response = null;
            Task.Run(async () =>
            {
                var httpClient = new HttpClient()
                {
                    BaseAddress = new Uri("http://localhost:32774/")
                };
                response = retryPolicy.ExecuteAsync(async () => await httpClient.GetAsync("API/LIBRARY")).Result;
                // Other code to execute after the awaited task completes
            }).Wait();

            if (response.IsSuccessStatusCode)
            {
                // Handle a successful response
            }
            else
            {
                // Handle a failed response
            }

            //return product;
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
