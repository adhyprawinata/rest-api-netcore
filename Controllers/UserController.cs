using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using rest_api.Models;
using MySql.Data.MySqlClient;
using System;

namespace rest_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<ResponseModel<IEnumerable<User>>> Get()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            List<User> users = new List<User>();
            ResponseModel<IEnumerable<User>> response = new ResponseModel<IEnumerable<User>>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "select * from user";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                User user = new User
                                {
                                    username = (string)reader["username"],
                                    password = (string)reader["password"]
                                };

                                users.Add(user);
                            }
                        }
                    }
                }

                response.Status = "Success";
                response.Message = "Data retrieved successfully";
                response.Data = users;
            }
            catch (Exception ex)
            {
                response.Status = "Error";
                response.Message = "An error occurred while retrieving data: " + ex.Message;
                response.Data = null;
            }

            return Ok(response);
        }
    }
}