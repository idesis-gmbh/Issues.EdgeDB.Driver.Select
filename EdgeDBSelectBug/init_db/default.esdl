module default {
    # Define an abstract type:
    abstract type HasImage {
        # just a URL to the image
        required property image -> str;
        index on (.image);
    }

    # Define a type extending from the abstract:
    type User extending HasImage {
        required property name -> str {
            # Ensure unique name for each User.
            constraint exclusive;
        }
    }

    # Define a type with constraints and defaults for properties:
    type Review {
        required property body -> str;
        required property rating -> int32 {
            constraint min_value(0);
            constraint max_value(5);
        }
        required property flag -> bool {
            default := False;
        }

        required link author -> User;
        required link movie -> Movie;

        required property creation_time -> datetime {
            default := datetime_current();
        }
    }

    # Define a type with a property that is computed from the combination of the other properties:
    type Person extending HasImage {
        required property first_name -> str {
            default := '';
        }
        required property middle_name -> str {
            default := '';
        }
        required property last_name -> str;
        property full_name :=
            (
                (
                    (.first_name ++ ' ')
                    if .first_name != '' else
                    ''
                ) ++
                (
                    (.middle_name ++ ' ')
                    if .middle_name != '' else
                    ''
                ) ++
                .last_name
            );
        property dob -> datetime;
        property dod -> datetime;
        property bio -> str;
    }

    # Define an abstract links:
    abstract link crew {
        # Provide a way to specify some "natural"
        # ordering, as relevant to the movie. This
        # may be order of importance, appearance, etc.
        property list_order -> int64;
    }

    abstract link directors extending crew;

    abstract link actors extending crew;

    # Define a type using abstract links and a computed property that aggregates values from another linked type:
    type Movie extending HasImage {
        required property title -> str;
        required property year -> int32;

        # Add an index for accessing movies by title and year,
        # separately and in combination.
        index on (.title);
        index on (.year);
        index on ((.title, .year));

        property description -> str;

        multi link directors extending crew -> Person;
        multi link actors extending crew -> Person;

        property avg_rating := math::mean(.<movie[is Review].rating);
    }

#    # Define an auto-incrementing scalar type and an object type using it as a property:
#    scalar type TicketNo extending sequence;
#
#    type Ticket {
#        property number -> TicketNo {
#            constraint exclusive;
#        }
#    }
}