using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Information
{
    public class Database
    {
        private string _ConnectionString = "";
        private SqlConnection _dbConnection = null;
        private SqlTransaction _dbTransaction = null;
        private bool _IsStartTran = false;
        //private string _ErrMsg = "";

        public Database(string ConnectionString)
        {
            this._ConnectionString = ConnectionString;
        }

        #region " GetSqlStringCommand "
        public DbCommand GetSqlStringCommand(string sSQL)
        {
            SqlCommand objCommand;
            try
            {
                objCommand = new SqlCommand(sSQL);
                return objCommand;
            }
            catch (Exception ex)
            {
                throw new Exception("GetSqlStringCommand:" + ex.Message);
            }
            finally
            {
                objCommand = null;
            }
        }
        #endregion

        #region " AddInParameter "
        public void AddInParameter(DbCommand dbCommand, string sParamNm, DbType sDBType, object oValue)
        {
            try
            {
                SqlParameter sqlParameter = new SqlParameter(sParamNm, sDBType);
                sqlParameter.Value = (oValue == null ? DBNull.Value : oValue);
                dbCommand.Parameters.Add(sqlParameter);
            }
            catch (Exception ex)
            {
                throw new Exception("AddInParameter:" + ex.Message);
            }
        }

        public void AddInParameter(SqlCommand dbCommand, string sParamNm, object oValue)
        {
            try
            {
                SqlParameter sqlParameter = new SqlParameter(sParamNm, oValue);
                dbCommand.Parameters.Add(sqlParameter);
            }
            catch (Exception ex)
            {
                throw new Exception("AddInParameter:" + ex.Message);
            }
        }
        #endregion

        #region " ExecuteDataSet "
        public DataSet ExecuteDataSet(DbCommand dbCommand)
        {
            SqlConnection objConn;
            SqlDataAdapter objSqlAdp;
            DataSet objDataSet;
            try
            {
                objConn = new SqlConnection(this._ConnectionString);

                dbCommand.Connection = objConn;
                //objSqlAdp.SelectCommand.CommandTimeout = this._DBConnTimeout;
                objDataSet = new DataSet();
                objConn.Open();
                objSqlAdp = new SqlDataAdapter((SqlCommand)dbCommand);
                objSqlAdp.Fill(objDataSet, "ResultTable");
                objConn.Close();
                return objDataSet;

            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteDataSet::" + ex.Message);
            }
            finally
            {
                objConn = null;
                objSqlAdp = null;
                objDataSet = null;
            }
        }
        #endregion

        #region " ExecuteNonQuery "

        public int ExecuteNonQuery(DbCommand dbCommand, SqlConnection dbConnection, SqlTransaction dbTransaction)
        {
            int iEffectCnt = 0;

            try
            {
                if (dbConnection == null)
                    _dbConnection = this.GetDBConnection();
                else
                    _dbConnection = dbConnection;

                if (_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();

                dbCommand.Connection = _dbConnection;

                if (dbTransaction != null)
                    dbCommand.Transaction = dbTransaction;

                iEffectCnt = dbCommand.ExecuteNonQuery();

                if (dbConnection == null)
                    _dbConnection.Close();

                return iEffectCnt;
            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteNonQuery::" + ex.Message);
            }
            finally
            {
            }
        }

        public int ExecuteNonQuery(DbCommand dbCommand, SqlConnection dbConnection)
        {
            int iEffectCnt = 0;
            try
            {
                dbCommand.Connection = dbConnection;
                if (this._IsStartTran == true)
                {
                    dbCommand.Transaction = this._dbTransaction;
                }
                //dbCommand.Connection.Open();
                iEffectCnt = dbCommand.ExecuteNonQuery();
                //dbCommand.Connection.Close();
                return iEffectCnt;
            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteNonQuery::" + ex.Message);
            }
            finally
            {
            }
        }

        public int ExecuteNonQuery(DbCommand dbCommand)
        {
            int iEffectCnt = 0;
            SqlConnection dbConnection;
            try
            {
                dbConnection = this.GetDBConnection();
                //dbConnection.Open();
                iEffectCnt = this.ExecuteNonQuery(dbCommand, dbConnection);
                dbConnection.Close();
                return iEffectCnt;
            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteNonQuery::" + ex.Message);
            }
            finally
            {
                dbConnection = null;
            }
        }
        #endregion

        #region " ExecuteScalar "
        public object ExecuteScalar(DbCommand dbCommand, SqlConnection dbConnection)
        {
            object oEffectCnt;
            try
            {
                dbCommand.Connection = dbConnection;
                if (this._IsStartTran == true)
                {
                    dbCommand.Transaction = this._dbTransaction;
                }
                //dbCommand.Connection.Open();
                oEffectCnt = dbCommand.ExecuteScalar();
                //dbCommand.Connection.Close();
                return oEffectCnt;
            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteNonQuery::" + ex.Message);
            }
            finally
            {
                //oEffectCnt = null;
            }
        }

        public object ExecuteScalar(DbCommand dbCommand)
        {
            object oEffectCnt;
            SqlConnection dbConnection;
            try
            {
                dbConnection = this.GetDBConnection();
                //dbConnection.Open();
                oEffectCnt = this.ExecuteScalar(dbCommand, dbConnection);
                dbConnection.Close();
                return oEffectCnt;
            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteNonQuery::" + ex.Message);
            }
            finally
            {
                dbConnection = null;
                oEffectCnt = null;
            }
        }
        #endregion

        #region " Propertys "
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }

        public SqlConnection dbConnection
        {
            get { return _dbConnection; }
            set { _dbConnection = value; }
        }

        public SqlTransaction dbTransaction
        {
            get { return _dbTransaction; }
            set { _dbTransaction = value; }
        }

        public bool IsStartTran
        {
            get { return _IsStartTran; }
        }

        #endregion

        #region " ExecSQLRetrieve "
        public DataTable ExecSQLRetrieve(string sSQL, Dictionary<string, SqlParameter> objParams)
        {
            DbCommand dbCommand;
            Dictionary<string, SqlParameter>.Enumerator objParamEnum;
            try
            {
                dbCommand = this.GetSqlStringCommand(sSQL.ToString());

                if (objParams != null)
                {
                    objParamEnum = objParams.GetEnumerator();
                    while (objParamEnum.MoveNext() == true)
                    {
                        this.AddInParameter(dbCommand, objParamEnum.Current.Key, DbType.String, objParamEnum.Current.Value);
                    }
                }
                return this.ExecuteDataSet(dbCommand).Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception("ExecSQLRetrieve::" + ex.Message);
            }
            finally
            {
                dbCommand = null;
            }
        }

        public DataTable ExecSQLRetrieve(string sSQL, Dictionary<string, string> objParams)
        {
            DbCommand dbCommand;
            Dictionary<string, string>.Enumerator objParamEnum;
            try
            {
                dbCommand = this.GetSqlStringCommand(sSQL.ToString());

                if (objParams != null)
                {
                    objParamEnum = objParams.GetEnumerator();
                    while (objParamEnum.MoveNext() == true)
                    {
                        this.AddInParameter(dbCommand, objParamEnum.Current.Key, DbType.String, objParamEnum.Current.Value);
                    }
                }
                return this.ExecuteDataSet(dbCommand).Tables[0];

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                dbCommand = null;
            }
        }

        public DataTable ExecSQLRetrieve(string sSQL)
        {
            DbCommand dbCommand;
            try
            {
                dbCommand = this.GetSqlStringCommand(sSQL.ToString());
                return this.ExecuteDataSet(dbCommand).Tables[0];

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                dbCommand = null;
            }
        }
        #endregion

        #region " GetDBConnection "
        public SqlConnection GetDBConnection(string sConnectionString)
        {
            SqlConnection dbConnection;
            try
            {
                dbConnection = new SqlConnection(sConnectionString);
                dbConnection.Open();
                return dbConnection;
            }
            catch (Exception ex)
            {
                throw new Exception("GetDBConnection::" + ex.Message);
            }
            finally
            {
                dbConnection = null;
            }
        }

        public SqlConnection GetDBConnection()
        {
            try
            {
                return this.GetDBConnection(this._ConnectionString);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
            }
        }
        #endregion

        #region " StartTransaction "
        public void StartTransaction(IsolationLevel TranLevel, string sTransationName)
        {
            try
            {
                _IsStartTran = true;
                this._dbConnection = this.GetDBConnection();
                this._dbTransaction = this._dbConnection.BeginTransaction(TranLevel, sTransationName);
            }
            catch (Exception ex)
            {
                throw new Exception("StartTransaction:" + ex.Message);
            }
            finally
            {

            }
        }
        #endregion

        #region " CommitTransaction "
        public void CommitTransaction()
        {
            try
            {
                this._dbTransaction.Commit();
                this._dbConnection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("CommitTransaction:" + ex.Message);
            }
            finally
            {
                this._dbConnection = null;
                _IsStartTran = false;
                this._dbTransaction = null;
            }
        }
        #endregion

        #region " RollbackTransaction "
        public void RollbackTransaction()
        {
            try
            {
                this._dbTransaction.Rollback();
                this._dbConnection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("RollbackTransaction:" + ex.Message);
            }
            finally
            {
                this._dbConnection = null;
                _IsStartTran = false;
                this._dbTransaction = null;
            }
        }
        #endregion

        #region " CreateConnection "
        public DbConnection CreateConnection()
        {
            try
            {
                return this.GetDBConnection();
            }
            catch (Exception ex)
            {
                throw new Exception("CreateConnection:" + ex.Message);
            }
            finally
            {

            }
        }
        #endregion

        #region " CloseConnection "
        public void CloseConnection()
        {
            try
            {
                if (_dbConnection != null)
                {
                    _dbConnection.Dispose();
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion 

        #region " Commit "
        public void Commit()
        {
            this.CommitTransaction();
        }
        #endregion

        #region " Rollback"
        public void Rollback()
        {
            this.RollbackTransaction();
        }
        #endregion
    }
}
