using System;

[Serializable]
public class Producto
{
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; } // "Producto Final" o "Materia Prima"
    public double Precio { get; set; }
    public int Cantidad { get; set; }
    public DateTime FechaIngreso { get; set; }
    public override string ToString()
    {
        return $"{Codigo} - {Nombre} - {Tipo} - {Precio} - {Cantidad}";
    }
}