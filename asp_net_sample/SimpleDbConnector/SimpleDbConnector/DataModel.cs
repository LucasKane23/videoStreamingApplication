using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace SimpleDbConnector
{
    public class DataModel
    {
        private ConnectionPool connection_pool = null;

        public DataModel(String connection_string, String provider_name, int pool_size)
        {
            this.connection_pool = new ConnectionPool(connection_string, provider_name, pool_size);
        }
 
        public DataModel(ConnectionPool connection_pool)
        {
            this.connection_pool = connection_pool;
        }

        public String ExecProcValue(String procedureName, Dictionary<String, object> procedureParameters, DbConnection connection = null)
        {
            var result = "";
            ConnectionPoolNode node = null;

            try
            {
                if (connection == null)
                {
                    node = connection_pool.GetNode();
                    connection = node.connection;
                }

                var cmd = MakeCommand(connection, procedureName, procedureParameters);

                DbParameter p_ = cmd.CreateParameter();
                p_.ParameterName = "@value";
                p_.Value = -1;
                p_.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p_);

                try
                {
                    cmd.Prepare();
                    DataTable dt = new DataTable();

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.FieldCount > 0)
                        {
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                DataColumn dc = new DataColumn(dr.GetName(i), dr.GetFieldType(i));
                                dt.Columns.Add(dc);
                            }
                            object[] rowobject = new object[dr.FieldCount];
                            while (dr.Read())
                            {
                                dr.GetValues(rowobject);
                                dt.LoadDataRow(rowobject, true);
                            }
                        }
                    }

                    result = cmd.Parameters["@value"].Value.ToString();
                }
                finally
                {
                    cmd.Dispose();
                }
            }
            finally
            {
                if (node != null)
                {
                    node.Free();
                }
            }
            return result;
        }

        public DataTable ExecProc(String procedureName, Dictionary<String, object> procedureParameters, DbConnection connection = null, Boolean silent = false)
        {
            ConnectionPoolNode node = null;
            DataTable dt = null;

            try
            {
                if (connection == null)
                {
                    node = connection_pool.GetNode();
                    connection = node.connection;
                }

                var cmd = MakeCommand(connection, procedureName, procedureParameters);
                
                try
                {
                    cmd.Prepare();

                    dt = new DataTable();
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.FieldCount > 0)
                        {
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                DataColumn dc = new DataColumn(dr.GetName(i), dr.GetFieldType(i));
                                dt.Columns.Add(dc);
                            }
                            object[] rowobject = new object[dr.FieldCount];
                            while (dr.Read())
                            {
                                dr.GetValues(rowobject);
                                dt.LoadDataRow(rowobject, true);
                            }
                        }
                    }
                }
                finally
                {
                    cmd.Dispose();
                }
            }
            finally
            {
                node.Free();
            }
            
            return dt;
        }

        private DbCommand MakeCommand(DbConnection connection, String procedure_name, Dictionary<String, object> parameters)
        {
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procedure_name;

            foreach (KeyValuePair<string, object> param in parameters)
            {
                DbParameter p = cmd.CreateParameter();
                p.ParameterName = param.Key;
                p.Value = param.Value;
                cmd.Parameters.Add(p);
            }

            return cmd;
        }
    }
}
