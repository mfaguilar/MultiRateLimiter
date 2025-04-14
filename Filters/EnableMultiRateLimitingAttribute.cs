namespace AppointmentsWeb.Controllers.Filters;


/// <summary>
/// Aplica múltiples políticas al endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class EnableMultiRateLimitingAttribute : Attribute
{
    public string[] PolicyNames { get; }

    public EnableMultiRateLimitingAttribute(params string[] policyNames)
    {
        if (policyNames == null || policyNames.Length == 0)
        {
            throw new ArgumentException("Tiene que indicarse al menos una Policy.", nameof(policyNames));
        }
        this.PolicyNames = policyNames;
    }
}