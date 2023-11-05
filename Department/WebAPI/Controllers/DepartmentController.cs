using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Department.Models;

namespace WebAPI.Department.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            DataTable table = new DataTable();
            var con = new NpgsqlConnection(connectionString: _configuration.GetConnectionString("StudAppConnection"));
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"Select  DepartmentId, DepartmentName from Department";
            NpgsqlDataReader reader = cmd.ExecuteReader();
            table.Load(reader);
            con.Close();
            return new JsonResult(table);
        }


        [HttpPost]
        public JsonResult Post(WebAPI.Department.Models.Department model)
        {
            var con = new NpgsqlConnection(connectionString: _configuration.GetConnectionString("StudAppConnection"));
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"INSERT INTO Department (DepartmentName) VALUES (@departmentName)";
            cmd.Parameters.AddWithValue("departmentName", model.DepartmentName);
            cmd.ExecuteNonQuery();
            con.Close();
            return new JsonResult("Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(WebAPI.Department.Models.Department model)
        {
            var con = new NpgsqlConnection(connectionString: _configuration.GetConnectionString("StudAppConnection"));
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"UPDATE Department" +
                                $" SET DepartmentName='{model.DepartmentName}'" +
                                $" WHERE DepartmentId = {model.DepartmentId}";
            cmd.ExecuteNonQuery();
            con.Close();
            return new JsonResult("Updated Successfully");
        }


        [HttpDelete("{id}")]
        public JsonResult Delete( int id)
        {


            var con = new NpgsqlConnection(connectionString: _configuration.GetConnectionString("StudAppConnection"));
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText =" Delete from Department  where DepartmentId = " + id;
            cmd.ExecuteNonQuery();
            con.Close();
            return new JsonResult("Deleted Successfully");
        }
    }
}
