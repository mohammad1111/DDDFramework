using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gig.Framework.Core.RuleEngine;
using Gig.Framework.Core.Settings;
using Microsoft.Data.SqlClient;

namespace Gig.Framework.Persistence.Ef;

public class RuleRepository : IRuleRepository
{
    public readonly string ConnectionString;
    public readonly string MicroServiceName;
    public readonly TimeSpan RuleExpireTime;

    public RuleRepository(IDataSetting dataSetting)
    {
        ConnectionString = dataSetting.WriteDataConnectionString;
        MicroServiceName = dataSetting.MicroServiceName;
        RuleExpireTime = TimeSpan.Parse(dataSetting.RuleExpireTime);
    }

    public async Task SaveRuleSet(RunningGigRuleSet ruleSet)
    {
        var command =
            $@"INSERT INTO {MicroServiceName}.RunningGigRuleSet (RuleSetId,ExpireTime,HandelRule,TypeOfData,IsRemoved,MicroServiceName) VALUES ('{ruleSet.RuleSetId}','{ruleSet.ExpireTime:yyyy-MM-dd hh:mm:ss}',0,N'{ruleSet.TypeOfData}',0,'{MicroServiceName}')";
        await using var sqlConnection = new SqlConnection(ConnectionString);
        try
        {
            await sqlConnection.OpenAsync();
            var sqlCommand = new SqlCommand(command, sqlConnection);

            await sqlCommand.ExecuteNonQueryAsync();
            await sqlConnection.CloseAsync();
        }
        catch (Exception)
        {
            await sqlConnection.CloseAsync();
            throw;
        }
    }

    public async Task AddRule(RunningGigRuleResult ruleResult)
    {
        await using var sqlConnection = new SqlConnection(ConnectionString);
        var command =
            $@"INSERT INTO {MicroServiceName}.RunningGigRuleResult(RuleSetId,RuleId,RuleContent,TypeOfData,IsRemoved, BusinessRuleId)VALUES('{ruleResult.RuleSetId}','{ruleResult.RuleId}','{ruleResult.RuleContent}','{ruleResult.TypeOfData}',0,{ruleResult.BusinessRuleId})";
        try
        {
            await sqlConnection.OpenAsync();
            var sqlCommand = new SqlCommand(command,
                sqlConnection);
            await sqlCommand.ExecuteNonQueryAsync();
            await sqlConnection.CloseAsync();
        }
        catch (Exception)
        {
            await sqlConnection.CloseAsync();
            throw;
        }
    }

    public async Task<IList<RunningGigRuleResult>> GetRuleResult(Guid ruleSetId)
    {
        var runningGigRules = new List<RunningGigRuleResult>();
        var sql =
            $@"SELECT RuleSetId,RuleId,RuleContent,TypeOfData,IsRemoved,BusinessRuleId FROM {MicroServiceName}.RunningGigRuleResult WHERE [RuleSetId]='{ruleSetId}' AND  IsRemoved=0";
        await using var sqlConnection = new SqlConnection(ConnectionString);
        try
        {
            await sqlConnection.OpenAsync();
            var sqlCommand = new SqlCommand(sql, sqlConnection);
            await using var dataReader = await sqlCommand.ExecuteReaderAsync();
            if (dataReader != null)
                while (dataReader.Read())
                {
                    var publisherEvent = new RunningGigRuleResult
                    {
                        RuleSetId = Guid.Parse(dataReader[0].ToString()),
                        RuleId = Guid.Parse(dataReader[1].ToString()),
                        RuleContent = dataReader[2].ToString(),
                        TypeOfData = dataReader[3].ToString(),
                        IsRemoved = bool.Parse(dataReader[4].ToString()),
                        BusinessRuleId = long.Parse(dataReader[5].ToString())
                    };

                    runningGigRules.Add(publisherEvent);
                }

            await sqlConnection.CloseAsync();
            return runningGigRules;
        }
        catch (Exception)
        {
            await sqlConnection.CloseAsync();
            throw;
        }
    }

    public async Task<IList<RunningGigRuleResult>> GetRulesResult(IEnumerable<Guid> rulesSetId)
    {
        var runningGigRules = new List<RunningGigRuleResult>();
        var sql =
            $@"SELECT RuleSetId,RuleId,RuleContent,TypeOfData,IsRemoved,BusinessRuleId FROM {MicroServiceName}.RunningGigRuleResult WHERE [RuleSetId] IN ('{string.Join("','", rulesSetId)}') AND  IsRemoved=0";
        await using var sqlConnection = new SqlConnection(ConnectionString);
        try
        {
            await sqlConnection.OpenAsync();
            var sqlCommand = new SqlCommand(sql, sqlConnection);
            await using var dataReader = await sqlCommand.ExecuteReaderAsync();
            if (dataReader != null)
                while (dataReader.Read())
                {
                    var publisherEvent = new RunningGigRuleResult
                    {
                        RuleSetId = Guid.Parse(dataReader[0].ToString()),
                        RuleId = Guid.Parse(dataReader[1].ToString()),
                        RuleContent = dataReader[2].ToString(),
                        TypeOfData = dataReader[3].ToString(),
                        IsRemoved = bool.Parse(dataReader[4].ToString()),
                        BusinessRuleId = long.Parse(dataReader[5].ToString())
                    };

                    runningGigRules.Add(publisherEvent);
                }

            await sqlConnection.CloseAsync();
            return runningGigRules;
        }
        catch (Exception)
        {
            await sqlConnection.CloseAsync();
            throw;
        }
    }

    public async Task<RunningGigRuleSet> GetRuleSet(Guid ruleSetId)
    {
        var runningGigRules = new List<RunningGigRuleSet>();
        var sql =
            $@"SELECT RuleSetId,ExpireTime,HandelRule,TypeOfData,MicroServiceName FROM {MicroServiceName}.RunningGigRuleSet WHERE [RuleSetId]='{ruleSetId}'";
        await using var sqlConnection = new SqlConnection(ConnectionString);
        try
        {
            await sqlConnection.OpenAsync();
            var sqlCommand = new SqlCommand(sql, sqlConnection);
            await using var dataReader = await sqlCommand.ExecuteReaderAsync();
            if (dataReader != null)
                while (dataReader.Read())
                {
                    var publisherEvent = new RunningGigRuleSet
                    {
                        RuleSetId = Guid.Parse(dataReader[0].ToString()),
                        ExpireTime = DateTime.Parse(dataReader[1].ToString()),
                        HandelRule = bool.Parse(dataReader[2].ToString()),
                        TypeOfData = dataReader[3].ToString(),
                        MicroServiceName = dataReader[4].ToString()
                    };

                    runningGigRules.Add(publisherEvent);
                }

            await sqlConnection.CloseAsync();
            return runningGigRules.FirstOrDefault();
        }
        catch (Exception)
        {
            await sqlConnection.CloseAsync();
            throw;
        }
    }

    public async Task RemoveRuleSet(Guid ruleSetId)
    {
        await using var sqlConnection = new SqlConnection(ConnectionString);
        var commandRule =
            $@"Update  {MicroServiceName}.RunningGigRuleSet  SET  IsRemoved=1  WHERE  [RuleSetId]='{ruleSetId}'";
        try
        {
            await sqlConnection.OpenAsync();
            var sqlCommandRule = new SqlCommand(commandRule, sqlConnection);
            await sqlCommandRule.ExecuteNonQueryAsync();
            await sqlConnection.CloseAsync();
        }
        catch (Exception)
        {
            await sqlConnection.CloseAsync();
            throw;
        }
    }

    public async Task RemoveRuleResult(Guid ruleId)
    {
        await using var sqlConnection = new SqlConnection(ConnectionString);
        var commandRuleResult =
            $@"Update  {MicroServiceName}.RunningGigRuleResult  SET  IsRemoved=1  WHERE RuleId='{ruleId}'";
        try
        {
            await sqlConnection.OpenAsync();
            var sqlCommandRuleResult = new SqlCommand(commandRuleResult, sqlConnection);

            await sqlCommandRuleResult.ExecuteNonQueryAsync();

            await sqlConnection.CloseAsync();
        }
        catch (Exception)
        {
            await sqlConnection.CloseAsync();
            throw;
        }
    }

    public async Task<IEnumerable<RunningGigRuleSet>> GetExpireRule()
    {
        var runningGigRules = new List<RunningGigRuleSet>();
        var sql =
            $@"SELECT RuleSetId,ExpireTime,HandelRule,TypeOfData,MicroServiceName FROM {MicroServiceName}.RunningGigRuleSet 
                              WHERE ExpireTime<'{DateTime.Now:yyyy-MM-dd hh:mm:ss}' AND HandelRule=0 And IsRemoved=0 AND [MicroServiceName]='{MicroServiceName}'";

        await using var sqlConnection = new SqlConnection(ConnectionString);
        try
        {
            await sqlConnection.OpenAsync();
            var sqlCommand = new SqlCommand(sql, sqlConnection);
            await using var dataReader = await sqlCommand.ExecuteReaderAsync();
            if (dataReader != null)
                while (dataReader.Read())
                {
                    var publisherEvent = new RunningGigRuleSet
                    {
                        RuleSetId = Guid.Parse(dataReader[0].ToString()),
                        ExpireTime = DateTime.Parse(dataReader[1].ToString()),
                        HandelRule = bool.Parse(dataReader[2].ToString()),
                        TypeOfData = dataReader[3].ToString(),
                        MicroServiceName = dataReader[4].ToString()
                    };
                    runningGigRules.Add(publisherEvent);
                }

            await sqlConnection.CloseAsync();
            return runningGigRules;
        }
        catch (Exception)
        {
            await sqlConnection.CloseAsync();
            throw;
        }
    }

    public async Task<IEnumerable<RunningGigRuleSet>> GetCommitRule()
    {
        var runningGigRules = new List<RunningGigRuleSet>();
        var sql =
            $@"SELECT RuleSetId,ExpireTime,HandelRule,TypeOfData,MicroServiceName FROM {MicroServiceName}.RunningGigRuleSet 
                              WHERE  HandelRule=1 And IsRemoved=0 AND [MicroServiceName]='{MicroServiceName}'";

        await using var sqlConnection = new SqlConnection(ConnectionString);
        try
        {
            await sqlConnection.OpenAsync();
            var sqlCommand = new SqlCommand(sql, sqlConnection);
            await using var dataReader = await sqlCommand.ExecuteReaderAsync();
            if (dataReader != null)
                while (dataReader.Read())
                {
                    var publisherEvent = new RunningGigRuleSet
                    {
                        RuleSetId = Guid.Parse(dataReader[0].ToString()),
                        ExpireTime = DateTime.Parse(dataReader[1].ToString()),
                        HandelRule = bool.Parse(dataReader[2].ToString()),
                        TypeOfData = dataReader[3].ToString(),
                        MicroServiceName = dataReader[4].ToString()
                    };
                    runningGigRules.Add(publisherEvent);
                }

            await sqlConnection.CloseAsync();
            return runningGigRules;
        }
        catch (Exception)
        {
            await sqlConnection.CloseAsync();
            throw;
        }
    }
}