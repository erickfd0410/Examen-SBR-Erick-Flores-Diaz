using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examen_SBR_IA
{
    //Ejercicio #11: Control de trafico ferroviario y Cambio de agujas
    internal class Program
    {
        static void Main(string[] args)
        {
            // Reglas
            var reglas = new List<(string Id, int Prio, string[] Ant, string[] Cons)>
        {
            ("R1", 65,  new[] {"TREN_APROXIMANDOSE", "SEÑAL_VERDE_ACTIVA"},new[] {"SOLICITUD_PASO"}),
            ("R2", 85,  new[] {"SOLICITUD_PASO", "VIA_PRINCIPAL_LIBRE", "TREN_PASAJEROS"},new[] {"AUTORIZAR_VIA_PRINCIPAL"}),
            ("R3", 75,  new[] {"SOLICITUD_PASO", "-TREN_PASAJEROS"},new[] {"DESVIAR_VIA_SECUNDARIA"}),
            ("R4", 100, new[] {"TREN_APROXIMANDOSE", "-SEÑAL_VERDE_ACTIVA"},new[] {"FRENADO_EMERGENCIA"})
        };
            // Casos de prueba
            var casos = new[]
            {
            new[] {"TREN_APROXIMANDOSE", "SEÑAL_VERDE_ACTIVA", "VIA_PRINCIPAL_LIBRE", "TREN_PASAJEROS"},
            new[] {"TREN_APROXIMANDOSE", "SEÑAL_VERDE_ACTIVA"},
            new[] {"TREN_APROXIMANDOSE"}
        };
            // Motor de inferencia
            for (int i = 0; i < casos.Length; i++)
            {
                Console.WriteLine($"\n CASO {i + 1}: {{{string.Join(", ", casos[i])}}}");
                var hechos = new HashSet<string>(casos[i], StringComparer.OrdinalIgnoreCase);
                var disparadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var secuencia = new List<string>();
                int paso = 1;

                while (true)
                {
                    var candidata = reglas
                        .Where(r => !disparadas.Contains(r.Id) && r.Ant.All(cond =>
                        {
                            bool negada = cond.StartsWith("-");
                            string varNombre = negada ? cond.Substring(1) : cond;
                            return negada ? !hechos.Contains(varNombre) : hechos.Contains(varNombre);
                        }))
                        .OrderByDescending(r => r.Prio)
                        .FirstOrDefault();
                    if (candidata.Id == null)
                    {
                        Console.WriteLine($"[Paso {paso}] Objetivo alcanzado.");
                        break;
                    }
                    disparadas.Add(candidata.Id);
                    secuencia.Add(candidata.Id);
                    foreach (var c in candidata.Cons) hechos.Add(c);

                    Console.WriteLine($"[Paso {paso++}] Dispara {candidata.Id} (P:{candidata.Prio}) -> Deduce: {string.Join(", ", candidata.Cons)}");
                }
                string resumen = secuencia.Count > 0 ? $"Encadena {string.Join(" -> ", secuencia)}" : "Sin disparos";
                Console.WriteLine($"Resumen: [{resumen}]");
                Console.WriteLine($"Hechos Finales: {{{string.Join(", ", hechos)}}}");
            }
        }
    }
}