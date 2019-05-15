using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace SimpleDbConnector
{
    public class ConnectionPool
    {
        public ConnectionPoolNode[] Pool = null;
        private String connection_string;
        private String provider_name;

        public ConnectionPool(String connection_string, String provider_name, int PoolSize)
        {
            this.connection_string = connection_string;
            this.provider_name = provider_name;
            if (PoolSize == 0)
            {
                Pool = null;
            }
            else
            {
                Pool = new ConnectionPoolNode[PoolSize];
            }
        }

        public ConnectionPoolNode GetNode()
        {
            if (Pool == null)
            {
                return new ConnectionPoolNode(this, -1);
            }
            int i = 0;
            var ep = DateTime.Now;
            while (true)
            {
                if (Pool[i] == null || (Pool[i] != null && !Pool[i].state))
                {
                    break;
                }

                i++;

                if (i >= Pool.Length)
                {
                    i = 0;
                    Thread.Sleep(30);
                }
            }
            lock (this)
            {
                if (Pool[i] == null)
                {
                    Pool[i] = new ConnectionPoolNode(this, i);
                }
                Pool[i].state = true;
            }
            Pool[i].Connect(this.connection_string, this.provider_name);
            return Pool[i];
        }
    }

    public class ConnectionPoolNode
    {
        private ConnectionPool pool = null;
        public int index = -1;
        public Boolean state = true;
        public DbConnection connection;

        public void Connect(String connection_string, String provider_name)
        {
            if (connection == null)
            {
                var factory = DbProviderFactories.GetFactory(provider_name);
                connection = factory.CreateConnection();
                connection.ConnectionString = connection_string;
            }
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public ConnectionPoolNode(ConnectionPool _pool, int i)
        {
            index = i;
            pool = _pool;
        }

        public void Free()
        {
            state = false;
        }
    }
}
