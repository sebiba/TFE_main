\version "2.16.0"  % necessary for upgrading to future LilyPond versions.

\header{
	title = "gamme"
	subtitle = "gamme"
}

	tagline = "c'est de moi"

\header{
	piece = "gamme"
}

\relative c' {
	\time 4/4
	\key c \major
	a b c d e f e d c b a b c d e f e d c b a
}
