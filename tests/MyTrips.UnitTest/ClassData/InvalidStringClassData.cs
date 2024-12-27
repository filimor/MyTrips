using System.Collections;

namespace MyTrips.UnitTest.ClassData;

public class InvalidStringClassData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [string.Empty];
        yield return [new string(" ")];
        yield return [new string('a', 256)];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}