using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Globalization;
using System.Runtime.ConstrainedExecution;

namespace ForexCalculatorApp;

public partial class MainWindow : Window
{
    private readonly PositionCalculator calculator = new PositionCalculator();
    private string Buy_OR_Sell = "Buy";

    private string currency_Button_name = "";

    public MainWindow()
    {
        InitializeComponent();
    }

    private void CurrentButton_Click(object sender, RoutedEventArgs e)
    {
        ResetCurrencyButtonsColor();
        if (sender is Button selectedButton)
        {
            selectedButton.Background = Brush.Parse("#7657FF");
            selectedButton.BorderBrush = Brush.Parse("#A78BFA");

            currency_Button_name = selectedButton.Name;
        }
    }

    private void ResetCurrencyButtonsColor()
    {
        foreach (var button in new[] { USD_Select_Button, EUR_Select_Button, GBP_Select_Button })
        {
            button.Background = Brush.Parse("#111111");
            button.BorderBrush = Brush.Parse("#111111");
        }
    }

    private void Risk_Slider_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        Risk_Text.Text = $"{Math.Round(e.NewValue, 2):0.##}%";
    }

    private void Reset_Pair_Buttons()
    {
        foreach (var button in new[] { AUD_USD_Select_Button, EUR_GBP_Select_Button, GBP_USD_Select_Button, EUR_USD_Select_Button })
            button.Background = Brush.Parse("#17171D");
    }

    private void PairButton_Click(object sender, RoutedEventArgs e)
    {
        Reset_Pair_Buttons();
        if (sender is not Button pairButton)
            return;

        pairButton.Background = Brush.Parse("#594a9b");
        switch (pairButton.Name)
        {
            case "AUD_USD_Select_Button": Closed_Selector_Text.Text = "AUD/USD"; Closed_Selector_Symbol.Text = "🇦🇺"; break;
            case "EUR_GBP_Select_Button": Closed_Selector_Text.Text = "EUR/GBP"; Closed_Selector_Symbol.Text = "🇬🇧🇪🇺"; break;
            case "GBP_USD_Select_Button": Closed_Selector_Text.Text = "GBP/USD"; Closed_Selector_Symbol.Text = "🇬🇧"; break;
            default: Closed_Selector_Text.Text = "EUR/USD"; Closed_Selector_Symbol.Text = "🇪🇺"; break;
        }
    }

    private void SellBuy_Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button selectedButton)
            return;

        if (selectedButton.Name == "Buy_Button")
        {
            Buy_Button.BorderBrush = Brush.Parse("#6EE7B7");
            Buy_Button.BorderThickness = new Avalonia.Thickness(1);
            Sell_Button.BorderThickness = new Avalonia.Thickness(0);
            Buy_OR_Sell = "Buy";
        }
        else if (selectedButton.Name == "Sell_Button")
        {
            Sell_Button.BorderBrush = Brush.Parse("#FF8A9A");
            Sell_Button.BorderThickness = new Avalonia.Thickness(1);
            Buy_Button.BorderThickness = new Avalonia.Thickness(0);
            Buy_OR_Sell = "Sell";
        }

        CalculatePips();
    }

    private void Calculate_PIPS(object? sender, TextChangedEventArgs e) => CalculatePips();

    private void CalculatePips()
    {
        if (!TryParse(Entry_Input.Text, out var entry) || !TryParse(SL_Input.Text, out var stopLoss) || !TryParse(TP_Input.Text, out var takeProfit))
        {
            PIP_Risk.Text = "0";
            PIP_Reward.Text = "0";
            return;
        }

        var stopPips = Buy_OR_Sell == "Buy"
            ? calculator.Buy_CalculateSLDistance(entry, stopLoss)
            : calculator.Sell_CalculateSLDistance(entry, stopLoss);
        var targetPips = Buy_OR_Sell == "Buy"
            ? calculator.Buy_CalculateTPDistance(entry, takeProfit)
            : calculator.Sell_CalculateTPDistance(entry, takeProfit);

        PIP_Risk.Text = $"{Math.Round(stopPips, 2)} pips";
        PIP_Reward.Text = $"{Math.Round(targetPips, 2)} pips";

        double rTr = Math.Round(targetPips / stopPips, 2);

        riskTOreward.Text = $"{stopPips / stopPips}: {rTr}";

    }

    private static bool TryParse(string? value, out double result) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result) ||
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);


    private void CalculateButtonClicked(object sender, RoutedEventArgs e)
    {
        double initialBalance = Convert.ToDouble(initalUserBalance.Text);
        double risk_percent = Convert.ToDouble(Risk_Slider.Value);
        double finalValue = Math.Round(calculator.calculateFinalRiskAmount(risk_percent, initialBalance), 2);

        double stopLossPIPs = Convert.ToDouble(PIP_Risk.Text.Replace(" pips", ""));

        if (currency_Button_name == "USD_Select_Button")
        {
            int pipValue = 10;
            FinalRiskAmount.Text = $"${finalValue}";
            string lots = Convert.ToString(calculator.CalculateLotSize(finalValue, stopLossPIPs, pipValue));
            FinalLotSize.Text = $"{lots} Lots";
        }
        else if (currency_Button_name == "GBP_Select_Button")
        {
            int pipValue = 8;
            FinalRiskAmount.Text = $"£{finalValue}";
            string lots = Convert.ToString(calculator.CalculateLotSize(finalValue, stopLossPIPs, pipValue));
            FinalLotSize.Text = $"{lots} Lots";
        }
        else if (currency_Button_name == "EUR_Select_Button")
        {
            int pipValue = 9;
            FinalRiskAmount.Text = $"€{finalValue}";
            string lots = Convert.ToString(calculator.CalculateLotSize(finalValue, stopLossPIPs, pipValue));
            FinalLotSize.Text = $"{lots} Lots";
        }

        StopDistanceText.Text = PIP_Risk.Text;

        Risk_TO_Reward_Text.Text = riskTOreward.Text;

        ENTRY_Text.Text = $"{Math.Round(Convert.ToDouble(Entry_Input.Text), 5)}";

        STOPLOSS_Text.Text = $"{Math.Round(Convert.ToDouble(SL_Input.Text), 5)}";

        TARGET_Text.Text = $"{Math.Round(Convert.ToDouble(TP_Input.Text), 5)}";
    }
    
}
