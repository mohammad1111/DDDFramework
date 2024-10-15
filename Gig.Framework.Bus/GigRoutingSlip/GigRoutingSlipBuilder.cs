using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gig.Framework.Core.Events;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;

namespace Gig.Framework.Bus.GigRoutingSlip;

public class GigRoutingSlipBuilder
{
    private readonly RoutingSlipBuilder _builder;

    public GigRoutingSlipBuilder(IEvent clientEvent, Guid trackingNumber)
    {
        _builder = new RoutingSlipBuilder(trackingNumber);
        _builder.AddVariable("BranchId", clientEvent.BranchId);
        _builder.AddVariable("CompanyId", clientEvent.CompanyId);
        _builder.AddVariable("UserId", clientEvent.UserId);
        _builder.AddVariable("SubSystemId", clientEvent.SubSystemId);
        _builder.AddVariable("LangTypeCode", clientEvent.LangTypeCode);
        _builder.AddVariable("IsAdmin", clientEvent.IsAdmin);
    }

    public GigRoutingSlipBuilder(IEvent clientEvent, RoutingSlip routingSlip,
        Func<IEnumerable<Activity>, IEnumerable<Activity>> activitySelector)
    {
        _builder = new RoutingSlipBuilder(routingSlip, activitySelector);

        _builder.AddVariable("BranchId", clientEvent.BranchId);
        _builder.AddVariable("CompanyId", clientEvent.CompanyId);
        _builder.AddVariable("UserId", clientEvent.UserId);
        _builder.AddVariable("SubSystemId", clientEvent.SubSystemId);
        _builder.AddVariable("IsAdmin", clientEvent.IsAdmin);
    }

    public GigRoutingSlipBuilder(IEvent clientEvent, RoutingSlip routingSlip, IEnumerable<Activity> itinerary,
        IEnumerable<Activity> sourceItinerary)
    {
        _builder = new RoutingSlipBuilder(routingSlip, itinerary, sourceItinerary);

        _builder.AddVariable("BranchId", clientEvent.BranchId);
        _builder.AddVariable("CompanyId", clientEvent.CompanyId);
        _builder.AddVariable("UserId", clientEvent.UserId);
        _builder.AddVariable("SubSystemId", clientEvent.SubSystemId);
        _builder.AddVariable("LangTypeCode", clientEvent.LangTypeCode);
        _builder.AddVariable("IsAdmin", clientEvent.IsAdmin);
    }

    public GigRoutingSlipBuilder(IEvent clientEvent, RoutingSlip routingSlip, IEnumerable<CompensateLog> compensateLogs)
    {
        _builder = new RoutingSlipBuilder(routingSlip, compensateLogs);
        _builder.AddVariable("BranchId", clientEvent.BranchId);
        _builder.AddVariable("CompanyId", clientEvent.CompanyId);
        _builder.AddVariable("UserId", clientEvent.UserId);
        _builder.AddVariable("SubSystemId", clientEvent.SubSystemId);
        _builder.AddVariable("LangTypeCode", clientEvent.LangTypeCode);
        _builder.AddVariable("IsAdmin", clientEvent.IsAdmin);
    }


    /// <summary>
    ///     The tracking number of the routing slip
    /// </summary>
    public Guid TrackingNumber => _builder.TrackingNumber;

    /// <summary>
    ///     Adds an activity to the routing slip without specifying any arguments
    /// </summary>
    /// <param name="name">The activity name</param>
    /// <param name="executeAddress">The execution address of the activity</param>
    public void AddActivity(string name, Uri executeAddress)
    {
        _builder.AddActivity(name, executeAddress);
    }

    /// <summary>
    ///     Adds an activity to the routing slip specifying activity arguments as an anonymous object
    /// </summary>
    /// <param name="name">The activity name</param>
    /// <param name="executeAddress">The execution address of the activity</param>
    /// <param name="arguments">An anonymous object of properties matching the argument names of the activity</param>
    public void AddActivity(string name, Uri executeAddress, object arguments)
    {
        _builder.AddActivity(name, executeAddress, arguments);
    }

    /// <summary>
    ///     Adds an activity to the routing slip specifying activity arguments a dictionary
    /// </summary>
    /// <param name="name">The activity name</param>
    /// <param name="executeAddress">The execution address of the activity</param>
    /// <param name="arguments">A dictionary of name/values matching the activity argument properties</param>
    public void AddActivity(string name, Uri executeAddress, IDictionary<string, object> arguments)
    {
        _builder.AddActivity(name, executeAddress, arguments);
    }

    /// <summary>
    ///     Add a string value to the routing slip
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddVariable(string key, string value)
    {
        _builder.AddVariable(key, value);
    }

    /// <summary>
    ///     Add an object variable to the routing slip
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddVariable(string key, object value)
    {
        _builder.AddVariable(key, value);
    }

    /// <summary>
    ///     Sets the value of any existing variables to the value in the anonymous object,
    ///     as well as adding any additional variables that did not exist previously.
    ///     For example, SetVariables(new { IntValue = 27, StringValue = "Hello, World." });
    /// </summary>
    /// <param name="values"></param>
    public void SetVariables(object values)
    {
        _builder.SetVariables(values);
    }

    public void SetVariables(IEnumerable<KeyValuePair<string, object>> values)
    {
        _builder.SetVariables(values);
    }

    /// <summary>
    ///     Adds the activities from the source itinerary to the new routing slip and removes them from the
    ///     source itinerary.
    /// </summary>
    /// <returns></returns>
    public int AddActivitiesFromSourceItinerary()
    {
        return _builder.AddActivitiesFromSourceItinerary();
    }

    /// <summary>
    ///     Add an explicit subscription to the routing slip events
    /// </summary>
    /// <param name="address">The destination address where the events are sent</param>
    /// <param name="events">The events to include in the subscription</param>
    public void AddSubscription(Uri address, RoutingSlipEvents events)
    {
        _builder.AddSubscription(address, events);
    }

    /// <summary>
    ///     Add an explicit subscription to the routing slip events
    /// </summary>
    /// <param name="address">The destination address where the events are sent</param>
    /// <param name="events">The events to include in the subscription</param>
    /// <param name="contents">The contents of the routing slip event</param>
    public void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents)
    {
        _builder.AddSubscription(address, events, contents);
    }

    /// <summary>
    ///     Add an explicit subscription to the routing slip events
    /// </summary>
    /// <param name="address">The destination address where the events are sent</param>
    /// <param name="events">The events to include in the subscription</param>
    /// <param name="contents">The contents of the routing slip event</param>
    /// <param name="activityName">Only send events for the specified activity</param>
    public void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents,
        string activityName)
    {
        _builder.AddSubscription(address, events, contents, activityName);
    }

    /// <summary>
    ///     Adds a message subscription to the routing slip that will be sent at the specified event points
    /// </summary>
    /// <param name="address"></param>
    /// <param name="events"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public Task AddSubscription(Uri address, RoutingSlipEvents events, Func<ISendEndpoint, Task> callback)
    {
        return _builder.AddSubscription(address, events, callback);
    }

    /// <summary>
    ///     Adds a message subscription to the routing slip that will be sent at the specified event points
    /// </summary>
    /// <param name="address"></param>
    /// <param name="events"></param>
    /// <param name="contents"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public Task AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents,
        Func<ISendEndpoint, Task> callback)
    {
        return _builder.AddSubscription(address, events, contents, callback);
    }

    /// <summary>
    ///     Adds a message subscription to the routing slip that will be sent at the specified event points
    /// </summary>
    /// <param name="address"></param>
    /// <param name="events"></param>
    /// <param name="activityName">Only send events for the specified activity</param>
    /// <param name="contents"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public Task AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents,
        string activityName,
        Func<ISendEndpoint, Task> callback)
    {
        return _builder.AddSubscription(address, events, contents, activityName, callback);
    }

    /// <summary>
    ///     Builds the routing slip using the current state of the builder
    /// </summary>
    /// <returns>The RoutingSlip</returns>
    public RoutingSlip Build()
    {
        return _builder.Build();
    }

    public void AddActivityLog(HostInfo host, string name, Guid activityTrackingNumber, DateTime timestamp,
        TimeSpan duration)
    {
        _builder.AddActivityLog(host, name, activityTrackingNumber, timestamp, duration);
    }

    public void AddCompensateLog(Guid activityTrackingNumber, Uri compensateAddress, object logObject)
    {
        _builder.AddCompensateLog(activityTrackingNumber, compensateAddress, logObject);
    }

    public void AddCompensateLog(Guid activityTrackingNumber, Uri compensateAddress,
        IDictionary<string, object> data)
    {
        _builder.AddCompensateLog(activityTrackingNumber, compensateAddress, data);
    }

    /// <summary>
    ///     Adds an activity exception to the routing slip
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name">The name of the faulted activity</param>
    /// <param name="activityTrackingNumber">The activity tracking number</param>
    /// <param name="timestamp">The timestamp of the exception</param>
    /// <param name="elapsed">The time elapsed from the start of the activity to the exception</param>
    /// <param name="exception">The exception thrown by the activity</param>
    public void AddActivityException(HostInfo host, string name, Guid activityTrackingNumber, DateTime timestamp,
        TimeSpan elapsed,
        Exception exception)
    {
        _builder.AddActivityException(host, name, activityTrackingNumber, timestamp, elapsed, exception);
    }

    /// <summary>
    ///     Adds an activity exception to the routing slip
    /// </summary>
    /// <param name="host"></param>
    /// <param name="name">The name of the faulted activity</param>
    /// <param name="activityTrackingNumber">The activity tracking number</param>
    /// <param name="timestamp">The timestamp of the exception</param>
    /// <param name="elapsed">The time elapsed from the start of the activity to the exception</param>
    /// <param name="exceptionInfo"></param>
    public void AddActivityException(HostInfo host, string name, Guid activityTrackingNumber, DateTime timestamp,
        TimeSpan elapsed,
        ExceptionInfo exceptionInfo)
    {
        _builder.AddActivityException(host, name, activityTrackingNumber, timestamp, elapsed, exceptionInfo);
    }

    public void AddActivityException(ActivityException activityException)
    {
        _builder.AddActivityException(activityException);
    }
}