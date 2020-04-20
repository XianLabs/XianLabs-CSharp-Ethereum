# XianLabs-CSharp-Ethereum
C# Ethereum Mainnet program for sending ERC-20 Tokens

Part of the Chimera project (ERC-20 Token).

Current progress: All good. Website backend currently pushing submissions to the DB, c# program is detecting them and filling the orders/transfers and marking them as complete (if they do complete)

Current improvements needed: Pending transactions can make the c# program get "stuck" forever. Add some way to check the estimated send time and increase gas if needed, or move onto the next one for now.
