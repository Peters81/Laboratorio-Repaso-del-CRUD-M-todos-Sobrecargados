namespace SobreCargaMetodos;

public class SobreCarga
{
    // prueba los métodos Cuadrados sobrecargados
    public void ProbarMetodosSobreCargados()
    {
        Console.WriteLine(
            "El cuadrado del integer 7 es {0}",
            Cuadrado(7)
        );

        Console.WriteLine(
            "El Cuadrado del double 7.5 es {0}",
            Cuadrado(7.5)
        );
    }

    public int Cuadrado(int valorInt)
    {
        Console.WriteLine(
            "Se llamó a Cuadrado con argumento int:{0}",
            valorInt
        );

        return valorInt * valorInt;
    }

    public double Cuadrado(double valorDouble)
    {
        Console.WriteLine(
            "Se llamó a Cuadrado con argumento double:{0}",
            valorDouble
        );

        return valorDouble * valorDouble;
    }
}