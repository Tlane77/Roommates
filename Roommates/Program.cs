using System;
using System.Collections.Generic;
using Roommates.Models;
using Roommates.Repositories;

namespace Roommates
{
    class Program
    {
        /// <summary>
        ///  This is the address of the database.
        ///  We define it here as a constant since it will never change.
        /// </summary>
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true";

        static void Main(string[] args)
        {
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);

            Console.WriteLine("Getting All Rooms:");
            Console.WriteLine();

            List<Room> allRooms = roomRepo.GetAll();

            foreach (Room room in allRooms)
            {
                Console.WriteLine($"{room.Id} {room.Name} {room.MaxOccupancy}");
            }

            Console.WriteLine("----------------------------");
            Console.WriteLine("Getting Room with Id 1");

            Room singleRoom = roomRepo.GetById(1);

            Console.WriteLine($"{singleRoom.Id} {singleRoom.Name} {singleRoom.MaxOccupancy}");




            Room bathroom = new Room
            {
                Name = "Bathroom",
                MaxOccupancy = 1
            };

            roomRepo.Insert(bathroom);

            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Added the new Room with id {bathroom.Id}");


            bathroom.MaxOccupancy = 3;
            roomRepo.Update(bathroom);

            Room bathroomFromDb = roomRepo.GetById(bathroom.Id);

            Console.WriteLine($"{bathroomFromDb.Id} {bathroomFromDb.Name} {bathroomFromDb.MaxOccupancy}");
            Console.WriteLine("-------------------------------");


            roomRepo.Delete(bathroom.Id);
            allRooms = roomRepo.GetAll();
            foreach (Room room in allRooms)

            {
                Console.WriteLine($"{room.Id} {room.Name} {room.MaxOccupancy}");
            }
            Console.WriteLine("-------------------------------");


            /////Implement the RoommateRepository class to include the following methods

            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);

            Console.WriteLine("Getting All Roommates:");
            Console.WriteLine();

            List<Roommate> allRoommates = roommateRepo.GetAll();

            foreach (Roommate roommate in allRoommates)
            {
                Console.WriteLine($"{roommate.Id} {roommate.Firstname} {roommate.Lastname} rent: {roommate.RentPortion}%, date moved in: {roommate.MovedInDate}");
            };

            Console.WriteLine("----------------------------");
            Console.WriteLine("Getting Roommate with Id 1");

            Roommate singleRoommate = roommateRepo.GetById(1);

            Console.WriteLine($"{singleRoommate.Id} {singleRoommate.Firstname} {singleRoommate.Lastname} rent: {singleRoommate.RentPortion}%, date moved in: {singleRoommate.MovedInDate}");

            Console.WriteLine("----------------------------");
            Console.WriteLine("Getting All Roommates with Room:");
            Console.WriteLine();

            List<Roommate> allRoommatesWithRoom = roommateRepo.GetAllWithRoom(1);

            foreach (Roommate roommate in allRoommatesWithRoom)
            {
                Console.WriteLine($"{roommate.Id} {roommate.Firstname} {roommate.Lastname} rent: {roommate.RentPortion}%, date moved in: {roommate.MovedInDate}");
            };

            Roommate newRoomate = new Roommate
            {
                Firstname = "Andy",
                Lastname = "Collins",
                RentPortion = 10,
                MovedInDate = new DateTime(2020, 11, 06),
                Room = allRooms[2]
            };
            roommateRepo.Insert(newRoomate);

            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Added the new Roommate with id {newRoomate.Id}");


            newRoomate.RentPortion = 20;
            roommateRepo.Update(newRoomate);
            allRoommates = roommateRepo.GetAll();
            Console.WriteLine("-----------------------------------------");
            foreach (Roommate roommate in allRoommates)
            {
                Console.WriteLine($"{roommate.Id} {roommate.Firstname} {roommate.Lastname} rent: {roommate.RentPortion}%, date moved in: {roommate.MovedInDate}");
            }

            roommateRepo.Delete(newRoomate.Id);
            allRoommates = roommateRepo.GetAll();
            Console.WriteLine("-----------------------------------------");
            foreach (Roommate roommate in allRoommates)
            {
                Console.WriteLine($"{roommate.Id} {roommate.Firstname} {roommate.Lastname} rent: {roommate.RentPortion}%, date moved in: {roommate.MovedInDate}");
            }
        }
    }
}
        

       
    




