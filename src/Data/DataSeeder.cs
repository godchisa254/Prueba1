using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Prueba1.src.Models;

namespace Prueba1.src.Data
{
    public class DataSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDBContext>();
                
                var rutsExistentesUsuarios = new HashSet<string>();

                if(!context.Usuarios.Any())
                {
                    var usuarioFaker = new Faker<Usuario>()
                        .RuleFor(p => p.Rut, f => GenerateUniqueRandomRut(rutsExistentesUsuarios))
                        .RuleFor(p => p.Nombre , f => f.Name.FullName())
                        .RuleFor(p => p.Email, f => f.PickRandom(new[] { "poleras", "pantalones", "sombreros"}))
                        .RuleFor(p => p.Genero, f => f.PickRandom(new[] { "masculino", "femenino", "otro", "prefiero no decirlo"}))
                        .RuleFor(p => p.FechaNacimiento , f => DateOnly.FromDateTime(f.Date.Past(100, DateTime.Now)));
                    
                    var usuarios = usuarioFaker.Generate(12);
                    context.Usuarios.AddRange(usuarios);
                    context.SaveChanges();
                }

                context.SaveChanges();
            }
        }
            
        private static string GenerateUniqueRandomRut(HashSet<string> rutsExistentesUsuarios)
        {
            string rut;
            do
            {
                rut = GenerateRandomRut();
            } while (rutsExistentesUsuarios.Contains(rut));
            rutsExistentesUsuarios.Add(rut);
            return rut;
        }

        private static string GenerateRandomRut()
        {
            Random random = new();
            int rutSinVerificador = random.Next(1, 99999999); 

            int verificador = CalcularDigitoVerificador(rutSinVerificador);
            string verificadorStr = verificador == 10 ? "K" : verificador.ToString();  

            return $"{rutSinVerificador}-{verificadorStr}";
        }

        private static int CalcularDigitoVerificador(int rutSinVerificador)
        {
            int suma = 0;
            int multiplicador = 2;

            while (rutSinVerificador > 0)
            {
                int digito = rutSinVerificador % 10;
                suma += digito * multiplicador;
                multiplicador = multiplicador == 7 ? 2 : multiplicador + 1;
                rutSinVerificador /= 10;
            }

            int resto = suma % 11;
            int verificador = 11 - resto;

            if (verificador == 11)
                return 0;   
            if (verificador == 10)
                return 10;  

            return verificador;
        }



    }
}
