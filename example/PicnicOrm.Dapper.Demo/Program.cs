﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

using PicnicOrm.Ado.Factories;
using PicnicOrm.Dapper.Demo.Factories;
using PicnicOrm.Dapper.Demo.Models;
using PicnicOrm.Dapper.Factories;
using PicnicOrm.Data;
using PicnicOrm.Mapping;

namespace PicnicOrm.Dapper.Demo
{
    class Program
    {
        #region Private Methods

        static void Main(string[] args)
        {
            //Map Address to User
            var userAddressMap = new OneToOneMapping<User, Address>(address => address.Id, user => user.AddressId, (user, address) => user.Address = address);

            //Map Address to Employer & Employer to User
            var employerAddressMap = new OneToOneMapping<Employer, Address>(address => address.Id, employer => employer.AddressId, (employer, address) => employer.Address = address);
            var userEmployerMap = new OneToOneMapping<User, Employer>(employer => employer.Id, user => user.EmployerId, (user, employer) => user.Employer = employer);
            userEmployerMap.AddMapping(employerAddressMap);

            //Map Car to user
            var userCarMap = new ManyToManyMapping<User, Car, UserCar>(car => car.Id, userCar => userCar.CarId, userCar => userCar.UserId,
                (user, cars) =>
                {
                    foreach (var car in cars)
                    {
                        user.Cars.Add(car);
                        car.Users.Add(user);
                    }
                });

            //User Mapping
            var userMap = new ParentMapping<User>(user => user.Id);
            userMap.AddMapping(userAddressMap);
            userMap.AddMapping(userEmployerMap);
            userMap.AddMapping(userCarMap);

            //var dataBroker = new SqlDataBroker(@"Server=(localdb)\ProjectsV12;Database=DemoDatabase;Integrated security=True", new DapperGridReaderFactory());
            var gridReaderFactory = new SqlGridReaderFactory();
            gridReaderFactory.AddEntityFactory(new AddressFactory());
            gridReaderFactory.AddEntityFactory(new CarFactory());
            gridReaderFactory.AddEntityFactory(new EmployerFactory());
            gridReaderFactory.AddEntityFactory(new UserFactory());
            gridReaderFactory.AddEntityFactory(new UserCarFactory());

            var dataBroker = new SqlDataBroker(@"Server=(localdb)\ProjectsV12;Database=DemoDatabase;Integrated security=True", gridReaderFactory);
            dataBroker.AddMapping(userMap);

            var totalTime = 0L;
            for (int i = 0; i < 100; i++)
            {
                Stopwatch watch = new Stopwatch();

                watch.Start();
                //var parameterList = new List<IDbParameter>();
                //parameterList.Add(new DbParameter("BirthDate", DateTime.Parse("06-01-1975"), DbType.Date));
                var users = dataBroker.ExecuteStoredProcedure<User>("dbo.ReadUser");
                watch.Stop();

                totalTime += watch.ElapsedMilliseconds;
            }

            var test = totalTime;
        }

        #endregion
    }
}