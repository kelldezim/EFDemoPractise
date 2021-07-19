using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EFDemoWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PeopleContext _db;

        public IndexModel(ILogger<IndexModel> logger, PeopleContext db)
        {
            _logger = logger;
            _db = db;
        }

        public void OnGet()
        {
            LoadSampleData();
            //retrieve all Person in People table and coresponding Addresses and EmailAddresses
            var people = _db.People.Include(a => a.Addresses)
                                    .Include(e => e.EmailAddresses)
                                    .Where(x => x.Age > 18 && x.Age < 65) // this method filter on serverside
                                    //.ToList()
                                    //.Where(x => AgeValidation(x.Age)); // filter on app side after retreving all People
                                    //Because c# code cannot be run on sql side instead of query
                                    // AND!!! before you run c# code on retrieved data it needs to be COLLECTION
                                    .ToList();
        }

        private bool AgeValidation(int age)
        {
            return (age > 18 && age < 65);
        }

        private void LoadSampleData()
        {
            if(_db.People.Count()  == 0)
            {
                string file = System.IO.File.ReadAllText("generated.json");
                var people = JsonSerializer.Deserialize<List<Person>>(file);
                _db.AddRange(people);
                _db.SaveChanges();
            }
        }
    }
}
