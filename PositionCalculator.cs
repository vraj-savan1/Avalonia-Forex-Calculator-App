using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using System;
using Avalonia.Rendering;

namespace ForexCalculatorApp;

public class PositionCalculator
{
    public void Main()
    {
        
    }

    public double Buy_CalculateSLDistance(double entry, double stopLoss)
    {
        double risk = (entry - stopLoss) * 10000;
        return risk;
    }

    public double Buy_CalculateTPDistance(double entry, double takeProfit)
    {
        double reward = (takeProfit - entry) * 10000;
        return reward;
    }

    public double Sell_CalculateSLDistance(double entry, double stopLoss)
    {
        double risk = (stopLoss - entry) * 10000;
        return risk;
    }

    public double Sell_CalculateTPDistance(double entry, double takeProfit)
    {
        double reward = (entry - takeProfit) * 10000;
        return reward;
    }

    public double calculateFinalRiskAmount(double riskPercentage, double initialBalance)
    {
        double riskAmount = riskPercentage / 100  * initialBalance;
        return riskAmount;
    }

    public double CalculateLotSize(double riskAmount, double Sl_Pips, int PipValue) {
        double lotSize = riskAmount / (Sl_Pips * PipValue);
        return Math.Round(lotSize, 2);
    }
}