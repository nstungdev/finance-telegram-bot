# Requirements

## Purpose
This API application is used as a webhook endpoint for a Telegram Bot.

## Features
- When a user sends `/gold` or `gold` in the Telegram Bot, the system returns the latest gold price.
- When a user sends `/usd` or `usd` in the Telegram Bot, the system returns the latest USD price.

## Response Format
- Telegram messages must be formatted as Markdown so the response looks clean and readable.
- Use visual icons/symbols to make the Telegram response more engaging and scannable.
- The response content must include:
	- Timestamp
	- Current price (today)
	- Trend (arrow symbol + increasing/decreasing/unchanged)
	- Yesterday's closing price

## Daily Price Scope
- Gold and USD prices are tracked at a daily level.
- The response must clearly show yesterday's closing price and the current price for comparison.

## Unit Standards
- Timestamp format: ISO 8601 UTC, for example 2026-03-23T08:15:00Z.
- Gold unit: VND per tael (VND/tael).
- Gold display format: 1,234,567 VND/tael.
- USD unit: VND per USD (VND/USD).
- USD display format: 25,430 VND/USD.
- Trend values:
	- ⬆️ increasing when CurrentPrice > YesterdayClose
	- ⬇️ decreasing when CurrentPrice < YesterdayClose
	- ➡️ unchanged when CurrentPrice = YesterdayClose
- Suggested icons:
	- Header: 🥇 for Gold, 💱 for USD
	- Timestamp: ⏱️
	- Yesterday Close: 📌
	- Current Price: 💵
	- Trend: 📊

## Telegram Markdown Response Template
Use parse mode Markdown (or MarkdownV2 if escaping is implemented) to keep message output clean.

Generic template with placeholders:

```markdown
*{AssetIcon} {AssetName} Price Update*

- ⏱️ *Timestamp:* {TimestampUtc}
- 📌 *Yesterday Close:* {YesterdayCloseFormatted}
- 💵 *Current Price:* {CurrentPriceFormatted}
- 📊 *Trend:* {TrendDisplay}
```

Gold example template:

```markdown
*🥇 Gold Price Update*

- ⏱️ *Timestamp:* {TimestampUtc}
- 📌 *Yesterday Close:* {GoldYesterdayCloseVndPerTael}
- 💵 *Current Price:* {GoldCurrentPriceVndPerTael}
- 📊 *Trend:* {GoldTrendDisplay}
```

USD example template:

```markdown
*💱 USD Price Update*

- ⏱️ *Timestamp:* {TimestampUtc}
- 📌 *Yesterday Close:* {UsdYesterdayCloseVndPerUsd}
- 💵 *Current Price:* {UsdCurrentPriceVndPerUsd}
- 📊 *Trend:* {UsdTrendDisplay}
```

## Placeholder Definitions
- {AssetName}: Gold or USD.
- {AssetIcon}: 🥇 for Gold, 💱 for USD.
- {TimestampUtc}: Timestamp in ISO 8601 UTC format.
- {YesterdayCloseFormatted}: Yesterday close with correct unit and thousands separators.
- {CurrentPriceFormatted}: Current price with correct unit and thousands separators.
- {TrendDisplay}: trend with arrow symbol, for example ⬆️ increasing, ⬇️ decreasing, ➡️ unchanged.
- {GoldYesterdayCloseVndPerTael}: Gold close value formatted as VND/tael.
- {GoldCurrentPriceVndPerTael}: Gold current value formatted as VND/tael.
- {GoldTrendDisplay}: Gold trend with arrow symbol.
- {UsdYesterdayCloseVndPerUsd}: USD close value formatted as VND/USD.
- {UsdCurrentPriceVndPerUsd}: USD current value formatted as VND/USD.
- {UsdTrendDisplay}: USD trend with arrow symbol.
