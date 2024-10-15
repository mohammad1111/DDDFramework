using System;
using System.Data;
using System.Threading.Tasks;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Settings;
using Microsoft.Data.SqlClient;

namespace Gig.Framework.Persistence.Ef;

public class UserAccessDataProvider : IUserAccessDataProvider
{
    private readonly IDataSetting _dataSetting;

    public UserAccessDataProvider(IDataSetting dataSetting)
    {
        _dataSetting = dataSetting;
    }

    public async Task<int> GetAccessLevel(long userId, long companyId, string[] permissions)
    {
        var dtPermissions = new DataTable();
        dtPermissions.Columns.Add("Text", typeof(string));
        foreach (var permission in permissions)
        {
            var row = dtPermissions.NewRow();
            row.BeginEdit();
            row["Text"] = permission.Trim();
            row.EndEdit();
            dtPermissions.Rows.Add(row);
        }


        var sqlConnection = new SqlConnection(_dataSetting.GlobalDataConnectionString);
        var sqlCommand = new SqlCommand("[sec].[usp_GngExGetUserAccessLevel]", sqlConnection)
            { CommandType = CommandType.StoredProcedure, CommandTimeout = 120 };
        sqlCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.BigInt)
            { Direction = ParameterDirection.Input, Value = userId });
        sqlCommand.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.BigInt)
            { Direction = ParameterDirection.Input, Value = companyId });
        sqlCommand.Parameters.Add(new SqlParameter("@ForUI", SqlDbType.Bit)
            { Direction = ParameterDirection.Input, Value = false });
        sqlCommand.Parameters.Add(new SqlParameter("@OperationNames", SqlDbType.Structured)
            { Direction = ParameterDirection.Input, TypeName = "[sec].[StringList]", Value = dtPermissions });

        try
        {
            sqlConnection.Open();
            var result = sqlCommand.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }
        catch
        {
            await sqlConnection.CloseAsync();
            throw;
        }
        finally
        {
            await sqlConnection.CloseAsync();
        }
    }
}