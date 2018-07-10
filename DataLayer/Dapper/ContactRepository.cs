using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Transactions;

namespace MicroOrmDemo.DataLayer.Dapper
{
    public class ContactRepository : IContactRepository
    {
        IDbConnection db = new SqlConnection(
            ConfigurationManager.
            ConnectionStrings["ContactsDb"]
            .ConnectionString);

        public Contact Add(Contact contact)
        {
            var sql =
    "INSERT INTO Contacts (FirstName, LastName, Email, Company, Title) VALUES(@FirstName, @LastName, @Email, @Company, @Title); " +
    "SELECT CAST(SCOPE_IDENTITY() as int)";
            var id = this.db.Query<int>(sql, contact).Single();
            contact.Id = id;
            return contact;
        }

        public Contact Find(int id)
        {
            return this.db.Query<Contact>("SELECT * FROM Contacts WHERE Id = @Id", new { Id = id }).SingleOrDefault();
        }

        public List<Contact> GetAll()
        {
            return this.db.Query<Contact>("SELECT * FROM Contacts").ToList();
        }

        public Contact GetFullContact(int id)
        {
            return this.db.Query<Contact>("SELECT * FROM Contacts WHERE Id = @Id", 
                new { Id = id }).FirstOrDefault();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public void Save(Contact contact)
        {
            using (var txScope = new TransactionScope())
            {
                //if (contact.IsNew)
                //{
                //    this.Add(contact);
                //}
                //else
                //{
                //    this.Update(contact);
                //}

                var parameters = new DynamicParameters();
                parameters.Add("@Id", value: contact.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@FirstName", contact.FirstName);
                parameters.Add("@LastName", contact.LastName);
                parameters.Add("@Company", contact.Company);
                parameters.Add("@Title", contact.Title);
                parameters.Add("@Email", contact.Email);
                this.db.Execute("SaveContact", parameters, commandType: CommandType.StoredProcedure);
                contact.Id = parameters.Get<int>("@Id");

                foreach (var addr in contact.Addresses.Where(a => !a.IsDeleted))
                {
                    addr.ContactId = contact.Id;

                    //if (addr.IsNew)
                    //{
                    //    this.Add(addr);
                    //}
                    //else
                    //{
                    //    this.Update(addr);
                    //}

                    var addrParams = new DynamicParameters(new
                    {
                        ContactId = addr.ContactId,
                        AddressType = addr.AddressType,
                        StreetAddress = addr.StreetAddress,
                        City = addr.City,
                        StateId = addr.StateId,
                        PostalCode = addr.PostalCode
                    });
                    addrParams.Add("@Id", addr.Id, DbType.Int32, ParameterDirection.InputOutput);
                    this.db.Execute("SaveAddress", addrParams, commandType: CommandType.StoredProcedure);
                    addr.Id = addrParams.Get<int>("@Id");
                }

                foreach (var addr in contact.Addresses.Where(a => a.IsDeleted))
                {
                    //this.db.Execute("DELETE FROM Addresses WHERE Id = @Id", new { addr.Id });
                    this.db.Execute("DeleteAddress", new { Id = addr.Id }, commandType: CommandType.StoredProcedure);
                }

                txScope.Complete();
            }
        }

        public Contact Update(Contact contact)
        {
            var sql =
                "UPDATE Contacts " +
                "SET FirstName = @FirstName," +
                "LastName = @LastName," +
                "Email = @Email," +
                "Company = @Company, " +
                "Title = @Title" +
                "WHERE Id = @Id";
            this.db.Execute(sql, contact);
            return contact;
        }
    }
}
