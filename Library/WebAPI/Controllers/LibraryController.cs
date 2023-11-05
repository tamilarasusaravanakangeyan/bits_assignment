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
using WebAPI.Library.Models;

namespace WebAPI.Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private IConfiguration _configuration;
        public LibraryController(IConfiguration configuration)
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
            cmd.CommandText = $"Select  BookId, BookName from Library";
            NpgsqlDataReader reader = cmd.ExecuteReader();
            table.Load(reader);
            con.Close();
            return new JsonResult(table);
        }


        [HttpPost]
        public JsonResult Post(WebAPI.Library.Models.Library model)
        {
            var con = new NpgsqlConnection(connectionString: _configuration.GetConnectionString("StudAppConnection"));
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"INSERT INTO Library (BookName) VALUES (@bookName)";
            cmd.Parameters.AddWithValue("bookName", model.BookName);
            cmd.ExecuteNonQuery();
            con.Close();
            return new JsonResult("Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(WebAPI.Library.Models.Library model)
        {
            var con = new NpgsqlConnection(connectionString: _configuration.GetConnectionString("StudAppConnection"));
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"UPDATE Library" +
                                $" SET BookName='{model.BookName}'" +
                                $" WHERE BookId = {model.BookId}";
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
            cmd.CommandText = " Delete from Library  where BookId = " + id;
            cmd.ExecuteNonQuery();
            con.Close();
            return new JsonResult("Deleted Successfully");
        }
    }
}
