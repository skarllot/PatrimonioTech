# Product Overview

PatrimonioTech is a Brazilian personal finance desktop application for managing investment portfolios and assisting with annual income tax (Imposto de Renda) declarations.

## Core Capabilities

- **Asset Management**: Track financial assets on the B3 exchange (stocks, ETFs, FIIs, BDRs) identified by ticker symbols (e.g., PETR4, KNRI11).
- **User Credentials**: Multi-user support with secure, password-derived encryption keys (PBKDF2) for per-user data isolation.
- **Portfolio Tracking**: Monitor investment positions across asset types (ordinary shares, preferred shares, units, funds, BDRs).
- **Tax Declaration Support**: Provide data organized to support annual Brazilian IR declaration workflows.

## Target Use Cases

- Individual Brazilian investors managing B3 equity portfolios who need a local, offline-first tool.
- Users preparing annual Imposto de Renda declarations who need organized asset history.

## Value Proposition

Local-first desktop application with no cloud dependency. Data is stored in an embedded database (LiteDB) on the user's machine. Secure per-user encryption derived from password at login time.

---
_Focus on patterns and purpose, not exhaustive feature lists_
