\version "2.16.0"  % necessary for upgrading to future LilyPond versions.

\header{
	title = "test"
	subtitle = "test"
}

tagline = "pure"

\header{
	piece = "test"
}

\relative c' {
	\time 6/8
	\key c \major
	d'8 b4 b8 b4 a8 g8 b8 d8 e4
}
