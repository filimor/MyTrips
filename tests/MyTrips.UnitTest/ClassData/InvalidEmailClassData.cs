using System.Collections;

namespace MyTrips.UnitTest.ClassData;

public class InvalidEmailClassData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return ["plainaddress"];
        yield return ["plainaddress.com"];
        yield return [".user@example.com"];
        yield return ["user@"];
        yield return ["user@.com"];
        yield return ["user@ex(ample).com"];
        yield return ["user@ex<ample>.com"];
        yield return ["@example.com"];
        yield return ["user@.com"];
        yield return ["@.com"];
        yield return ["user@example,com"];
        yield return ["user @example.com"];
        yield return ["user@ example.com"];
        yield return [string.Empty];
        yield return [new string(" ")];
        yield return [new string('a', 256)];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}