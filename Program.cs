using Microsoft.EntityFrameworkCore;
using PortfolioBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SETUP DATABASE ---
// Pastikan koneksi ini sesuai dengan yang Anda pakai (SQL Server atau SQLite).
// Jika sebelumnya pakai SQLite, ganti .UseSqlServer menjadi .UseSqlite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. DAFTARKAN CORS ---
// Kita ubah jadi "AllowAll" agar aman saat diakses via Ngrok (Public)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin() // Boleh diakses dari mana saja (termasuk Ngrok)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ==========================================
// BAGIAN PENTING: AGAR REACT BISA TAMPIL
// ==========================================

// 1. Agar browser otomatis membuka index.html saat akses root (/)
app.UseDefaultFiles(); 

// 2. Agar file CSS, JS, dan Gambar di folder wwwroot bisa dibaca
app.UseStaticFiles(); 

// ==========================================

// --- 3. AKTIFKAN CORS ---
app.UseCors("AllowAll"); 

app.UseAuthorization();

app.MapControllers();

// ==========================================
// BAGIAN PENTING: ROUTING REACT
// ==========================================
// Jika route tidak ditemukan di API, arahkan kembali ke index.html (untuk React Router)
app.MapFallbackToFile("index.html");
// ==========================================

app.Run();