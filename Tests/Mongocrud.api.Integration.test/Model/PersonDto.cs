using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mongocrud.api.Integration.test.Model
{
    public class PersonDto
    {
        public List<PersonDbModeldto> Results { get; set; }

        

    }




    public class PersonDbModeldto
    {


        public string Gender { get; set; }
        public Name Name { get; set; }
        public Location Location { get; set; }

        private string _email;
        public string Email { get => _email; set => _email = value.ToLower(); }

        public Login Login { get; set; }
        public Dob Dob { get; set; }
        public Dob Registered { get; set; }
        public string Phone { get; set; }
        public string Cell { get; set; }
        public Id Id { get; set; }
        public Picture Picture { get; set; }
        public string Nat { get; set; }



        public PersonDbModeldto Clone()
        {

            return new PersonDbModeldto
            {
                Gender = this.Gender,
                Name = new Name
                {
                    First = this.Name.First,
                    Last = this.Name.Last,
                },
                Location = this.Location,
                Email = this.Email,
                Login = this.Login,
                Dob = new Dob
                {
                    Age = this.Dob.Age,
                    Date = this.Dob.Date
                },
                Registered = this.Registered,
                Phone = this.Phone,
                Cell = this.Cell,
                Id = this.Id,
                Picture = this.Picture,
                Nat = this.Nat
               
        
            };


        }

    }

    public class Dob
    {
        public DateTimeOffset Date { get; set; }
        public int Age { get; set; }
    }

    public class Id
    {
        public string Name { get; set; }
        public string? Value { get; set; }
    }

    public class Location
    {
        public Street Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int Postcode { get; set; }
        public Coordinates Coordinates { get; set; }
        public Timezone Timezone { get; set; }
    }

    public class Coordinates
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class Street
    {
        public int Number { get; set; }
        public string Name { get; set; }
    }

    public class Timezone
    {
        public string Offset { get; set; }
        public string Description { get; set; }
    }

    public partial class Login
    {
        public Guid Uuid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Md5 { get; set; }
        public string Sha1 { get; set; }
        public string Sha256 { get; set; }
    }

    public class Name
    {
        public string? Title { get; set; }

        public string First { get; set; }
        public string Last { get; set; }
    }

    public class Picture
    {
        public Uri Large { get; set; }
        public Uri Medium { get; set; }
        public Uri Thumbnail { get; set; }
    }
}
