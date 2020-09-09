/// How to create a roommate repository...( Add a class.cs file to child repository).

using System;
using Microsoft.Data.SqlClient;//Automatic pr
using Roommates.Models;
using System.Collections.Generic;


namespace Roommates.Repositories//This is just a naming convention 
{
    /// <summary>
    ///  This class is responsible for interacting with Room data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    public class RoommateRepository : BaseRepository ///This is a child inheriting from the parent...
    {
        /// <summary>
        ///  When new RoomRespository is instantiated, pass the connection string along to the BaseRepository
        /// </summary>
        public RoommateRepository(string connectionString) : base(connectionString) { } //Inheritance

            ///baseRepository(momma) this is where the babies get their connection..
            ///Use F5 to run..
            ///

        // ...We'll add some methods shortly...

        /// <summary>
        ///  Get a list of all Rooms in the database
        /// </summary>
        
       
        public List<Roommate> GetAll()
        {
            //This is the first READ the READ has a close...INSERT,UPDATE AND DELETE ARE open() but never closed?
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            using (SqlConnection conn = Connection)
            {
                // //This is the where the tunnel is being drilled out ( it's locked on both sides so we know we need an open)
                // Note, we must Open() the connection, the "using" block doesn't do that for us.
                conn.Open();

                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Now we send a command..
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = "SELECT Id, FirstName, LastName, RentPortion, MoveInDate FROM Roommate; ";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the rooms we retrieve from the database.
                    List<Roommate> roommates = new List<Roommate>();

                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                        //  For our query, "Id" has an ordinal value of 0 and "Name" is 1.
                        int idColumnPosition = reader.GetOrdinal("Id");

                        // We user the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);

                        int firstnameColumnPosition = reader.GetOrdinal("FirstName");
                        string firstnameValue = reader.GetString(firstnameColumnPosition);

                        int lastnameColumnPosition = reader.GetOrdinal("LastName");
                        string lastnameValue = reader.GetString(lastnameColumnPosition);

                        int rentPortionPosition = reader.GetOrdinal("RentPortion");
                        int rentPortionValue = reader.GetInt32(rentPortionPosition);

                        int MoveInDatePosition = reader.GetOrdinal("MoveInDate");
                        DateTime MoveinDateValue = reader.GetDateTime(MoveInDatePosition);

                        // Now let's create a new room object using the data from the database.
                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            Firstname = firstnameValue,
                            Lastname = lastnameValue,
                            RentPortion = rentPortionValue,
                            MovedInDate = MoveinDateValue,
                            Room = null,
                        };

                        // ...and add that room object to our list.
                        roommates.Add(roommate);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of rooms who whomever called this method.
                    return roommates;
                }
            }
        }

        /// <summary>
        ///  Returns a single room with the given id.
        /// </summary>
        /// 
        //
        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT FirstName, LastName, RentPortion, MoveInDate FROM RoomMate WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Roommate roommate = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        roommate = new Roommate()
                        {
                            Id = id,
                            Firstname = reader.GetString(reader.GetOrdinal("FirstName")),
                            Lastname = reader.GetString(reader.GetOrdinal("LastName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            MovedInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                            Room = new Room()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("RoomId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy")),
                            }
                        };
                    }

                    reader.Close();
                    return roommate;
                }
            }
        }

        public List<Roommate> GetAllWithRoom(int roomId)
        {
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block doesn't do that for us.
                conn.Open();

                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = "SELECT rm.Id, FirstName, LastName, RentPortion, MoveInDate, r.Name as 'RoomName', rm.RoomId, r.MaxOccupancy FROM Roommate rm JOIN Room r on rm.roomId = r.Id WHERE roomId = @roomId;";
                    cmd.Parameters.AddWithValue("@roomId", roomId);

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the rooms we retrieve from the database.
                    List<Roommate> roommatesWithRoom = new List<Roommate>();

                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                        //  For our query, "Id" has an ordinal value of 0 and "Name" is 1.
                        int idColumnPosition = reader.GetOrdinal("Id");

                        // We user the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);

                        int firstnameColumnPosition = reader.GetOrdinal("FirstName");
                        string firstnameValue = reader.GetString(firstnameColumnPosition);

                        int lastnameColumnPosition = reader.GetOrdinal("LastName");
                        string lastnameValue = reader.GetString(lastnameColumnPosition);

                        int rentPortionPosition = reader.GetOrdinal("RentPortion");
                        int rentPortionValue = reader.GetInt32(rentPortionPosition);

                        int MoveInDatePosition = reader.GetOrdinal("MoveInDate");
                        DateTime MoveinDateValue = reader.GetDateTime(MoveInDatePosition);



                        // Now let's create a new room object using the data from the database.
                        Roommate roommatewithRoom = new Roommate
                        {
                            Id = idValue,
                            Firstname = firstnameValue,
                            Lastname = lastnameValue,
                            RentPortion = rentPortionValue,
                            MovedInDate = MoveinDateValue,

                            Room = new Room()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("RoomId")),
                                Name = reader.GetString(reader.GetOrdinal("RoomName")),
                                MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))

                            }

                        };

                        // ...and add that room object to our list.
                        roommatesWithRoom.Add(roommatewithRoom);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of rooms who whomever called this method.
                    return roommatesWithRoom;
                }
            }
        }

        /// <summary>
        ///  Add a new room to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void Insert(Roommate roommate)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // These SQL parameters are annoying. Why can't we use string interpolation?
                    // ... sql injection attacks!!!
                    cmd.CommandText = @"INSERT INTO Roommate (FirstName, LastName, RentPortion, MoveInDate, RoomId ) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@FirstName, @LastName, @RentPortion, @MoveInDate, @RoomId)";
                    cmd.Parameters.AddWithValue("@FirstName", roommate.Firstname);
                    cmd.Parameters.AddWithValue("@LastName", roommate.Lastname);
                    cmd.Parameters.AddWithValue("@RentPortion", roommate.RentPortion);
                    cmd.Parameters.AddWithValue("@MoveInDate", roommate.MovedInDate);
                    cmd.Parameters.AddWithValue("@RoomId", roommate.Room.Id);
                    int id = (int)cmd.ExecuteScalar();

                    roommate.Id = id;
                }
            }

            // when this method is finished we can look in the database and see the new room.
        }

        /////Update the Program.Main method to print a report of all roommates and their rooms

        /// <summary>
        ///  Updates the room
        /// </summary>
        public void Update(Roommate roommate)///VOID means nothing is returned here
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                 
                    cmd.CommandText = @"UPDATE Roommate
                                    SET Firstname = @firstName, Lastname = @lastName, RentPortion = @rentPortion, MovedInDate = @moveInDate, RoomId = @roomId
                                    WHERE Id = @id";
                    //ADD Parameters to UPDATE on
                    cmd.Parameters.AddWithValue("@id", roommate.Id);
                    cmd.Parameters.AddWithValue("@firstName", roommate.Firstname);
                    cmd.Parameters.AddWithValue("@lastName", roommate.Lastname);
                    cmd.Parameters.AddWithValue("@rentPortion", roommate.RentPortion);
                    cmd.Parameters.AddWithValue("@moveInDate", roommate.MovedInDate);
                    cmd.Parameters.AddWithValue("@RoomId", roommate.Room.Id);

                    cmd.ExecuteNonQuery();///Execute(RUN)
                }
            }
        }

        /// <summary>
        ///  Delete the room with the given id
        /// </summary>
        public void Delete(int id) //VOID because it does not return anything
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Roommate WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);//Pararmeters are needed to say WHERE to delete.
                    cmd.ExecuteNonQuery();//Run it and give me back the results
                }
            }
        }

    }
}