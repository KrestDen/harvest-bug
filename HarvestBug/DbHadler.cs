using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;

namespace HarvestBug
{
    class DbHadler
    {
        private SQLiteConnection m_connection;

        public DbHadler(string dbPath)
        {
            m_connection = new SQLiteConnection("Data Source=" + dbPath + "; Version=3;");
            try
            {
                m_connection.Open();

                if (!IsTableExist("credantials"))
                {
                    SQLiteCommand cmd = m_connection.CreateCommand();
                    string sqlCommand = "DROP TABLE IF EXISTS credantials;"
                        + "CREATE TABLE credantials("
                        + "id INTEGER PRIMARY KEY AUTOINCREMENT, "
                        + "user_name TEXT, "
                        + "password TEXT, "
                        + "token TEXT);";

                    cmd.CommandText = sqlCommand;
                    cmd.ExecuteNonQuery();
                    cmd.ExecuteNonQuery();
                }

                if (!IsTableExist("spam"))
                {
                    SQLiteCommand cmd = m_connection.CreateCommand();
                    string sqlCommand = "DROP TABLE IF EXISTS spam;"
                        + "CREATE TABLE spam("
                        + "id INTEGER PRIMARY KEY AUTOINCREMENT, "
                        + "user_id TEXT, "
                        + "message_sent TEXT, "
                        + "timestamp TEXT);";

                    cmd.CommandText = sqlCommand;
                    cmd.ExecuteNonQuery();
                    cmd.ExecuteNonQuery();
                }

                if (!IsTableExist("tasks"))
                {
                    SQLiteCommand cmd = m_connection.CreateCommand();
                    string sqlCommand = "DROP TABLE IF EXISTS tasks;"
                        + "CREATE TABLE tasks("
                        + "id INTEGER PRIMARY KEY AUTOINCREMENT, "
                        + "user_id TEXT, "
                        + "task_type TEXT, "
                        + "text TEXT, "
                        + "additional TEXT, "
                        + "max_reiteration TEXT, "
                        + "current_reiteretion TEXT);";

                    cmd.CommandText = sqlCommand;
                    cmd.ExecuteNonQuery();
                    cmd.ExecuteNonQuery();
                }

            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

          //  m_connection.Close();
        }

        ~DbHadler()
        {
        }

        public void AddTask(string botLogin, TaskData taskData)
        {
            SQLiteCommand cmd2 = m_connection.CreateCommand();
            string sqlCommandinsertRow = "INSERT INTO tasks(user_id, task_type, text, current_reiteretion, additional) "
                + "VALUES ('" + botLogin + "', '" + taskData.type.ToString() + "', '" + taskData.text + "', '0', '" +  taskData.attachments + "'); ";
            cmd2.CommandText = sqlCommandinsertRow;
            cmd2.ExecuteNonQuery();
            cmd2.Dispose();
        }

        public void ChangeMaxValue(string login, string max)
        {
            SQLiteCommand cmd3 = m_connection.CreateCommand();
            try
            {                
                string sqlCommand = "UPDATE tasks "
                    + "SET max_reiteration='" + max + "' WHERE user_id ='" + login + "';";
                cmd3.CommandText = sqlCommand;
                cmd3.ExecuteNonQuery();                
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cmd3.Dispose();
        }

        public string GetSavedToken(string userName)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT user_name, token FROM credantials";
            string token = "";
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (r["user_name"].ToString() == userName)
                    {
                        token = r["token"].ToString();
                        break;
                    }      
                }
                r.Close();
                cmd.Dispose();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return token;
        }

        public void SaveToken(string userName, string pass, string token)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            string sqlCommand = "";
            if (IsBotSaved(userName, pass))
            {                
                sqlCommand = "UPDATE credantials "+ "SET token='" + token + "' WHERE user_name ='" + userName + "' AND password = '" + pass + "'; ";
            }
            else
            {
                sqlCommand = "INSERT INTO credantials(user_name, password, token) "
                        + "VALUES ('" + userName + "', '" + pass + "', '" + token + "'); ";
            }
            cmd.CommandText = sqlCommand;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void UpdateTaskCounter(string botLogin)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            try
            {                
                cmd.CommandText = "SELECT current_reiteretion FROM tasks WHERE user_id = '" + botLogin + "';";
                SQLiteDataReader r = cmd.ExecuteReader();
                string currentCount = "";
                r.Read();
                var cur = r["current_reiteretion"];
                if (cur != null)
                {
                    currentCount = cur.ToString();
                }

                int currentReiteration = 0;
                if (currentCount != "")
                {
                    currentReiteration = Convert.ToUInt16(currentCount);
                }
                
                ++currentReiteration;

                cmd.Dispose();
                cmd = m_connection.CreateCommand();
                cmd.CommandText = "UPDATE tasks " + "SET current_reiteretion='" + currentReiteration.ToString() + "' WHERE user_id ='" + botLogin + "'; ";
                cmd.ExecuteNonQuery();               
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
            cmd.Dispose();
        }

        public void ResetCounters()
        {
            SQLiteCommand cmd3 = m_connection.CreateCommand();
            try
            {                
                string sqlCommand = "UPDATE tasks SET current_reiteretion='0', max_reiteration = '0';";
                cmd3.CommandText = sqlCommand;
                cmd3.ExecuteNonQuery();
                
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            cmd3.Dispose();
        }

        private bool IsBotSaved(string userName, string pass)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM credantials WHERE user_name = '" + userName + "' AND password = '" + pass + "';";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            return count > 0;
        }

        public string GetPhotoId(string albumUrl)
        {
            string photoId = "";
            try
            {
                if (IsAlbumPresent(albumUrl))
                {
                    SQLiteCommand cmd = m_connection.CreateCommand();
                    cmd.CommandText = "SELECT photo_id FROM albums WHERE album_url = '" + albumUrl + "';";
                    SQLiteDataReader r = cmd.ExecuteReader();
                    if (r != null)
                    {
                        while (r.Read())
                        {
                            photoId = r["photo_id"].ToString();
                        }
                        r.Close();
                    }
                    cmd.Dispose();
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return photoId;
        }
        /*
        public void UpdateInfo(string albumUrl, string groupeId, string nextPhotoId, string photoUrl, string date)
        {
            try
            {
                if (!IsAlbumPresent(albumUrl))
                {
                    SQLiteCommand cmd2 = m_connection.CreateCommand();
                    string sqlCommandinsertRow = "INSERT INTO albums(album_url, groupe_id, photo_id, photo_url, date) "
                        + "VALUES ('" + albumUrl + "', '" + groupeId + "', '" + nextPhotoId + "', '" + photoUrl + "', '" + date + "'); ";
                    cmd2.CommandText = sqlCommandinsertRow;
                    cmd2.ExecuteNonQuery();
                    cmd2.Dispose();
                }
                else
                {
                    SQLiteCommand cmd3 = m_connection.CreateCommand();
                    string sqlCommand = "UPDATE albums "
                        + "SET groupe_id='" + groupeId + "', photo_id = '" + nextPhotoId + "', photo_url='" + photoUrl + "', date='" + date + "' WHERE album_url ='" + albumUrl + "';";
                    cmd3.CommandText = sqlCommand;
                    cmd3.ExecuteNonQuery();
                    cmd3.Dispose();
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */
        public void MarkUserAsDone(string userId)
        {
            SQLiteCommand cmd3 = m_connection.CreateCommand();
            string sqlCommand = "UPDATE spam "
                + "SET message_sent='1' WHERE user_id ='"  + userId + "';";
            cmd3.CommandText = sqlCommand;
            cmd3.ExecuteNonQuery();
            cmd3.Dispose();
        }

        public void AddUsersForSpam(List<string> userIds, string sent)
        {
            SQLiteCommand cmd2 = m_connection.CreateCommand();
            foreach (string id in userIds)
            {
                if (!UserExist(id))
                {                    
                    string sqlCommandinsertRow = "INSERT INTO spam(user_id, message_sent) "
                        + "VALUES ('" + id + "', '" + sent + "');";
                    cmd2.CommandText = sqlCommandinsertRow;
                    cmd2.ExecuteNonQuery();
                }
            }
            cmd2.Dispose();
        }

        public void AddUsersForSpam(List<string> userIds, string sent, MessageSendPresenter presenter)
        {
            SQLiteCommand cmd2 = m_connection.CreateCommand();
            foreach (string id in userIds)
            {
                if (!UserExist(id))
                {
                    string sqlCommandinsertRow = "INSERT INTO spam(user_id, message_sent) "
                        + "VALUES ('" + id + "', '" + sent + "');";
                    cmd2.CommandText = sqlCommandinsertRow;
                    cmd2.ExecuteNonQuery();
                }
                else
                {
                    presenter.NewMessage("Already exist " + id);
                }
            }
            cmd2.Dispose();
        }

        public void GetUsersList(ref Dictionary<string, string> users)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT id, user_id FROM spam";
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                while (r.Read())
                {
                    users.Add(r["id"].ToString(), r["user_id"].ToString());
                }
                r.Close();
                cmd.Dispose();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GetTasks(ref Dictionary<string, TaskData> tasks)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT user_id, text, additional, max_reiteration, current_reiteretion FROM tasks";
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                while (r.Read())
                {
                    TaskData task = new TaskData();
                    task.text = r["text"].ToString();
                    task.attachments = r["additional"].ToString();
                    task.max = r["max_reiteration"].ToString();
                    task.current = r["current_reiteretion"].ToString();
                    tasks.Add(r["user_id"].ToString(), task);
                }
                r.Close();
                cmd.Dispose();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GetBotsList(ref List<string> bots)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT user_name FROM credantials";
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                while (r.Read())
                {
                    bots.Add(r["user_name"].ToString());
                }
                r.Close();
                cmd.Dispose();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Replace(string id, string userId)
        {
            SQLiteCommand cmd3 = m_connection.CreateCommand();
            string sqlCommand = "UPDATE spam "
                + "SET user_id ='" + userId + "' WHERE id ='" + id + "';";
            cmd3.CommandText = sqlCommand;
            cmd3.ExecuteNonQuery();
            cmd3.Dispose();
        }

        public void GetUsersForSpam(ref List<string> userIds)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT user_id FROM spam WHERE sent = '0'";
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                while (r.Read())
                {
                    userIds.Add(r["user_id"].ToString());
                }
                r.Close();
                cmd.Dispose();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string GetNextUserIDForSpam()
        {
            /*
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT user_id FROM spam WHERE sent = '0'  LIMIT 1";
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string userId = r["user_id"].ToString();
               
                r.Close();
                cmd.Dispose();
                if (userId == null || userId == "")
                {
                    throw new Exception("No users for spam");
                }
                return userId;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "";
            */
            return /*"romanovromahkin"12287641*/"205164899";
        }

        public bool UserExist(string id)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM spam WHERE user_id = '" + id + "';";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            return count > 0;
        }

        public void GetBusyBots(ref List<string> BusyBots)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT user_id FROM tasks";
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                while (r.Read())
                {
                    BusyBots.Add( r["user_id"].ToString());
                }
                r.Close();
                cmd.Dispose();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AddNewAlbum(string albumUrl)
        {
            //todo
            if (!IsAlbumPresent(albumUrl))
            {
                SQLiteCommand cmd2 = m_connection.CreateCommand();
                string sqlCommandinsertRow = "INSERT INTO albums(album_url) "
                    + "VALUES ('" + albumUrl + "');";
                cmd2.CommandText = sqlCommandinsertRow;
                cmd2.ExecuteNonQuery();
                cmd2.Dispose();
            }
        }

        public bool IsAlbumPresent(string albumUrl)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM albums WHERE album_url = '" + albumUrl + "';";
            int albumExist = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            return albumExist > 0;
        }

        public void GetListOfBots(out List<string> bolLogins)
        {
            bolLogins = new List<string>();
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT album_url FROM credantials";
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                while (r.Read())
                {
                    bolLogins.Add(r["user_name"].ToString());
                }
                r.Close();
                cmd.Dispose();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GetListOfTokens(ref List<string> tokens)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT token FROM credantials";
            try
            {
                SQLiteDataReader r = cmd.ExecuteReader();
                string line = String.Empty;
                while (r.Read())
                {
                    tokens.Add(r["token"].ToString());
                }
                r.Close();
                cmd.Dispose();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsTableExist(string tableName)
        {
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT (*) name FROM sqlite_master WHERE type = 'table' AND name = '" + tableName + "'";
            int tableCount = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            return tableCount > 0;
        }
    }
}
