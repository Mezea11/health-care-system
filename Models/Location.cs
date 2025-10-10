namespace App;
class Location
{
    public string Region { get; set; }
    public string HospitalName { get; set; }

    public Location(string region, string hospitalName)
    {
        Region = region;
        HospitalName = hospitalName;
    }

    public override string ToString()
    {
        return $"{HospitalName} ({Region})";
    }
}
