using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Konscious.Security.Cryptography;

namespace InterviewEasy.Identity.Infrastructure.Services;

public sealed class Argon2PasswordHasher : IPasswordHasher, IDisposable
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int MemoryKb = 65536;   // 64 MB
    private const int Iterations = 3;
    private const int Parallelism = 4;

    private readonly SemaphoreSlim _throttle;

    public Argon2PasswordHasher()
    {
        var maxConcurrency = Math.Max(2, Environment.ProcessorCount);
        _throttle = new SemaphoreSlim(maxConcurrency, maxConcurrency);
    }

    public string Hash(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password is required.", nameof(password));

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = ComputeHash(password, salt);

        return $"argon2id$v=19$m={MemoryKb},t={Iterations},p={Parallelism}$" +
               $"{Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        var parts = hash.Split('$');
        if (parts.Length != 6) return false;

        try
        {
            var salt = Convert.FromBase64String(parts[4]);
            var expectedHash = Convert.FromBase64String(parts[5]);
            var actualHash = ComputeHash(password, salt);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> HashAsync(string password, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password is required.", nameof(password));

        await _throttle.WaitAsync(ct);
        try
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = ComputeHash(password, salt);
            return $"argon2id$v=19$m={MemoryKb},t={Iterations},p={Parallelism}$" +
                   $"{Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }
        finally
        {
            _throttle.Release();
        }
    }

    public async Task<bool> VerifyAsync(string password, string hash, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        await _throttle.WaitAsync(ct);
        try
        {
            return Verify(password, hash);
        }
        finally
        {
            _throttle.Release();
        }
    }

    private static byte[] ComputeHash(string password, byte[] salt)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = Parallelism,
            MemorySize = MemoryKb,
            Iterations = Iterations
        };
        return argon2.GetBytes(HashSize);
    }

    public void Dispose() => _throttle.Dispose();
}
