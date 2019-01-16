using System;
using System.Collections.Generic;
using System.Text;
using ACorns.Hawkeye.Core.Utils.Accessors;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Specialized;
using ACorns.Hawkeye.Utils;

namespace ACorns.Hawkeye.Tabs.ProcessInfo
{
	public class SqlPoolInfo : IAllowSelect
	{
		private List<object> connections = new List<object>();
		private object dbConnectionPoolGroup;
		private string poolName;

		private static FieldAccesor poolGroupOptionsField;
		private static FieldAccesor poolCollectionField;
		private static FieldAccesor poolObjectListField;

		public SqlPoolInfo(string poolName, object dbConnectionPoolGroup)
		{
			this.poolName = poolName;
			this.dbConnectionPoolGroup = dbConnectionPoolGroup;

			InitAccessors();
		}

		private void InitAccessors()
		{
			if (poolGroupOptionsField == null)
			{
				poolGroupOptionsField = new FieldAccesor(dbConnectionPoolGroup, "_poolGroupOptions");
				poolCollectionField = new FieldAccesor(dbConnectionPoolGroup, "_poolCollection");
			}
		}

		private void InitPoolAccessors(object pool)
		{
			if (poolObjectListField == null && pool != null)
			{
				poolObjectListField = new FieldAccesor(pool, "_objectList");
			}
		}

		#region Properties
		public List<object> Connections
		{
			get
			{
				this.connections = new List<object>();

				HybridDictionary poolConnections = poolCollectionField.Get(dbConnectionPoolGroup) as HybridDictionary;
				if (poolConnections != null)
				{
					foreach (DictionaryEntry poolEntry in poolConnections)
					{
						object pool = poolEntry.Value;
						InitPoolAccessors(pool);
						ICollection connectionsInPool = poolObjectListField.Get(pool) as ICollection;
						if (connectionsInPool != null)
						{
							foreach (object conn in connectionsInPool)
							{
								if (conn != null)
								{
									this.connections.Add(conn);
								}
							}
						}
					}
				}

				return this.connections;
			}
		}
		public string PoolName
		{
			get { return this.poolName; }
		}
		public object PoolGroupOptions
		{
			get { return poolGroupOptionsField.Get(dbConnectionPoolGroup); }
		}
		#endregion

		public override string ToString()
		{
			return connections.Count + " in pool:" + poolName;
		}
	}
	//public class SqlConnectionInfo : IAllowSelect
	//{
	//    private object connection;
	//    public SqlConnectionInfo(object connection)
	//    {
	//        this.connection = connection;
	//    }

	//    public object Connection
	//    {

	//}
	public class SqlConnectionPoolInfo : IAllowSelect
	{
		private List<SqlPoolInfo> pools = new List<SqlPoolInfo>();

		private object sqlConnectionFactory;
		private IDictionary _connectionPoolGroups;
		
		public SqlConnectionPoolInfo()
		{
			GetPoolInfo();
		}

		public int PoolCount
		{
			get
			{
				return pools.Count;
			}
		}

		public IList Pools
		{
			get { return pools; }
		}


		private void GetPoolInfo()
		{
			Type SqlConnectionFactoryType = null;
			if (sqlConnectionFactory == null)
			{
				SqlConnectionFactoryType = typeof(SqlConnection).Assembly.GetType("System.Data.SqlClient.SqlConnectionFactory");
				FieldAccesor SingletonInstanceField = new FieldAccesor(SqlConnectionFactoryType, "SingletonInstance");
				sqlConnectionFactory = SingletonInstanceField.Get();
			}
			if (sqlConnectionFactory != null)
			{
				//private Dictionary<string, DbConnectionPoolGroup> _connectionPoolGroups;
				FieldAccesor _connectionPoolGroupsField = new FieldAccesor(SqlConnectionFactoryType, "_connectionPoolGroups");
				_connectionPoolGroupsField.Target = sqlConnectionFactory;
				_connectionPoolGroups = _connectionPoolGroupsField.Get() as IDictionary;
			}
			if (_connectionPoolGroups != null)
			{
				foreach (string key in _connectionPoolGroups.Keys)
				{
					SqlPoolInfo pool = new SqlPoolInfo(key, _connectionPoolGroups[key]);
					pools.Add(pool);
				}
			}
		}

		public override string ToString()
		{
			return "Pools: " + pools.Count;
		}
	}
}
