# LL(1) Parser (narrowly focused)

## Project Overview
This project implements a complete **LL(1) parser** for custom grammars, featuring:
- Automated computation of `FIRST` and `FOLLOW` sets
- LL(1) parse table generation
- Table-driven parsing algorithm
- Basic error recovery mechanisms

## Core Components

### Grammar Processing
- Strict LL(1) grammar compliance
- Algorithmic calculation of essential sets:
  - `FIRST` sets for all non-terminals
  - `FOLLOW` sets for parsing context
- Conflict detection for non-LL(1) grammars
