public struct WeaponData
{
    public string weaponName;
    public int damageAmount;
    public int range;
    public int capacity;
    public float activeTime;
    public float lockingStep;
    public string iconPath;
    public string particleSystemPath;
    public string prefabPath;

    public WeaponData(string name, int damage, int weaponRange, int weaponCapacity, 
        float timeActive, float lockStep, string path, string effectPath, string missilePrefab)
    {
        weaponName = name;
        damageAmount = damage;
        range = weaponRange;
        capacity = weaponCapacity;
        activeTime = timeActive;
        lockingStep = lockStep;
        iconPath = path;
        particleSystemPath = effectPath;
        prefabPath = missilePrefab;
    }
}