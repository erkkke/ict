using System.Data;
using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Example2
{
    class ContactDB : DataAccessLayer, IDisposable
    {
        SQLiteConnection con = default(SQLiteConnection);
        string cs = @"URI=file:test.db";
        public ContactDB()
        {
            con = new SQLiteConnection(cs);
            con.Open();
            PrepareDB();
        }

        public void Dispose()
        {
            con.Close();
        }

        private void ExecuteNonQuery(string commandText)
        {
            var cmd = new SQLiteCommand(con);
            cmd.CommandText = commandText;
            cmd.ExecuteNonQuery();
        }

        private void PrepareDB()
        {
            //SQLiteConnection.CreateFile("test.db");
            /*ExecuteNonQuery("DROP TABLE IF EXISTS contacts");
            ExecuteNonQuery("CREATE TABLE contacts(id STRING PRIMARY KEY, name TEXT, phone TEXT, address TEXT)");*/
        }

        public string CreateContact(ContactDTO contact)
        {
            string text = string.Format("INSERT INTO contacts VALUES('{0}', '{1}', '{2}', '{3}')",
                contact.Id,
                contact.Name,
                contact.Phone,
                contact.Addr);

            ExecuteNonQuery(text);
            return contact.Id;
        }

        public string EditContact(ContactDTO contact)
        {
            string text = string.Format("UPDATE contacts SET name='{1}', phone='{2}', address='{3}' WHERE id='{0}'",
                contact.Id,
                contact.Name,
                contact.Phone,
                contact.Addr);

            ExecuteNonQuery(text);

            return contact.Id;
        }

        public bool DeleteContact(string id)
        {
            string text = "DELETE FROM contacts WHERE id='" + id + "'";
            ExecuteNonQuery(text);
            return true;
        }

        public List<ContactDTO> GetAllContactsInPage(int offset, int limit, string searchingName, bool sort)
        {
            string sorting = "ORDER BY name";
            if (!sort)
                sorting = "";

            string selectSQL = string.Format("SELECT * FROM contacts WHERE name LIKE '{2}%' {3} LIMIT '{1}' OFFSET '{0}'",
                offset,
                limit,
                searchingName,
                sorting);

            return selectFromDB(selectSQL);
        }

        public ContactDTO GetContactById(string id)
        {
            return null;
        }


        public List<ContactDTO> selectFromDB(string selectSql)
        {
            List<ContactDTO> res = new List<ContactDTO>();
            using (SQLiteCommand command = new SQLiteCommand(selectSql, con))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var item = new ContactDTO
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Phone = reader.GetString(2),
                        Addr = reader.GetString(3)
                    };

                    res.Add(item);
                }
            }
            return res;
        }
    }
}
