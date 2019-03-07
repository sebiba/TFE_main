\version "2.16.0"  % necessary for upgrading to future LilyPond versions.

\header{
	title = "test"
	subtitle = "045"
}

tagline = "pure"

\header{
	piece = "Hello World!"
}

\relative c' {
	\time 4/4
	\key c \major
	 bes16 ees8 d8 b4 a16 g16 b16 d16 e2
}
