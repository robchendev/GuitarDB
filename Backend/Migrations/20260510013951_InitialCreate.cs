using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by_id = table.Column<int>(type: "integer", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_users_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_countries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "guitar_body_shapes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guitar_body_shapes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "guitar_bracing_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guitar_bracing_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "guitar_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guitar_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "material_constructions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_material_constructions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "material_grades",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_material_grades", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "materials",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_materials", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "revision_metadatas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<int>(type: "integer", nullable: false),
                    parent_revision_id = table.Column<int>(type: "integer", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by_id = table.Column<int>(type: "integer", nullable: true),
                    updated_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_revision_metadatas", x => x.id);
                    table.ForeignKey(
                        name: "fk_revision_metadatas_revision_metadatas_parent_revision_id",
                        column: x => x.parent_revision_id,
                        principalTable: "revision_metadatas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_revision_metadatas_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_revision_metadatas_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    country_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.id);
                    table.ForeignKey(
                        name: "fk_brands_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "luthiers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    country_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_luthiers", x => x.id);
                    table.ForeignKey(
                        name: "fk_luthiers_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "guitars",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    luthier_id = table.Column<int>(type: "integer", nullable: true),
                    brand_id = table.Column<int>(type: "integer", nullable: true),
                    country_id = table.Column<string>(type: "text", nullable: true),
                    guitar_type_id = table.Column<int>(type: "integer", nullable: true),
                    guitar_body_shape_id = table.Column<int>(type: "integer", nullable: true),
                    guitar_bracing_type_id = table.Column<int>(type: "integer", nullable: true),
                    number_of_strings = table.Column<int>(type: "integer", nullable: true),
                    number_of_frets = table.Column<int>(type: "integer", nullable: true),
                    scale_length = table.Column<decimal>(type: "numeric", nullable: true),
                    is_left_handed = table.Column<bool>(type: "boolean", nullable: true),
                    weight_grams = table.Column<int>(type: "integer", nullable: true),
                    fingerboard_material_id = table.Column<int>(type: "integer", nullable: true),
                    fingerboard_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    fingerboard_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    neck_material_id = table.Column<int>(type: "integer", nullable: true),
                    neck_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    neck_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    top_material_id = table.Column<int>(type: "integer", nullable: true),
                    top_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    top_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    back_material_id = table.Column<int>(type: "integer", nullable: true),
                    back_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    back_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    sides_material_id = table.Column<int>(type: "integer", nullable: true),
                    sides_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    sides_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    bridge_material_id = table.Column<int>(type: "integer", nullable: true),
                    bridge_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    bridge_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    binding_material_id = table.Column<int>(type: "integer", nullable: true),
                    saddle_material_id = table.Column<int>(type: "integer", nullable: true),
                    nut_material_id = table.Column<int>(type: "integer", nullable: true),
                    bridge_pin_material_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guitars", x => x.id);
                    table.ForeignKey(
                        name: "fk_guitars_brands_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brands",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_guitar_body_shapes_guitar_body_shape_id",
                        column: x => x.guitar_body_shape_id,
                        principalTable: "guitar_body_shapes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_guitar_bracing_types_guitar_bracing_type_id",
                        column: x => x.guitar_bracing_type_id,
                        principalTable: "guitar_bracing_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_guitar_types_guitar_type_id",
                        column: x => x.guitar_type_id,
                        principalTable: "guitar_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_luthiers_luthier_id",
                        column: x => x.luthier_id,
                        principalTable: "luthiers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_constructions_back_material_construction_id",
                        column: x => x.back_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_constructions_bridge_material_construction",
                        column: x => x.bridge_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_constructions_fingerboard_material_constru",
                        column: x => x.fingerboard_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_constructions_neck_material_construction_id",
                        column: x => x.neck_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_constructions_sides_material_construction_",
                        column: x => x.sides_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_constructions_top_material_construction_id",
                        column: x => x.top_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_grades_back_material_grade_id",
                        column: x => x.back_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_grades_bridge_material_grade_id",
                        column: x => x.bridge_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_grades_fingerboard_material_grade_id",
                        column: x => x.fingerboard_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_grades_neck_material_grade_id",
                        column: x => x.neck_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_grades_sides_material_grade_id",
                        column: x => x.sides_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_material_grades_top_material_grade_id",
                        column: x => x.top_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_materials_back_material_id",
                        column: x => x.back_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_materials_binding_material_id",
                        column: x => x.binding_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_materials_bridge_material_id",
                        column: x => x.bridge_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_materials_bridge_pin_material_id",
                        column: x => x.bridge_pin_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_materials_fingerboard_material_id",
                        column: x => x.fingerboard_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_materials_neck_material_id",
                        column: x => x.neck_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_materials_nut_material_id",
                        column: x => x.nut_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_materials_saddle_material_id",
                        column: x => x.saddle_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_materials_sides_material_id",
                        column: x => x.sides_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitars_materials_top_material_id",
                        column: x => x.top_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "luthier_revision",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    luthier_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    country_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_luthier_revision", x => x.id);
                    table.ForeignKey(
                        name: "fk_luthier_revision_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_luthier_revision_luthiers_luthier_id",
                        column: x => x.luthier_id,
                        principalTable: "luthiers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_luthier_revision_revision_metadatas_id",
                        column: x => x.id,
                        principalTable: "revision_metadatas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guitar_revisions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    guitar_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    luthier_id = table.Column<int>(type: "integer", nullable: true),
                    brand_id = table.Column<int>(type: "integer", nullable: true),
                    country_id = table.Column<string>(type: "text", nullable: true),
                    guitar_type_id = table.Column<int>(type: "integer", nullable: true),
                    guitar_body_shape_id = table.Column<int>(type: "integer", nullable: true),
                    guitar_bracing_type_id = table.Column<int>(type: "integer", nullable: true),
                    number_of_strings = table.Column<int>(type: "integer", nullable: true),
                    number_of_frets = table.Column<int>(type: "integer", nullable: true),
                    scale_length = table.Column<decimal>(type: "numeric", nullable: true),
                    is_left_handed = table.Column<bool>(type: "boolean", nullable: true),
                    weight_grams = table.Column<int>(type: "integer", nullable: true),
                    fingerboard_material_id = table.Column<int>(type: "integer", nullable: true),
                    fingerboard_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    fingerboard_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    neck_material_id = table.Column<int>(type: "integer", nullable: true),
                    neck_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    neck_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    top_material_id = table.Column<int>(type: "integer", nullable: true),
                    top_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    top_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    back_material_id = table.Column<int>(type: "integer", nullable: true),
                    back_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    back_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    sides_material_id = table.Column<int>(type: "integer", nullable: true),
                    sides_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    sides_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    bridge_material_id = table.Column<int>(type: "integer", nullable: true),
                    bridge_material_construction_id = table.Column<int>(type: "integer", nullable: true),
                    bridge_material_grade_id = table.Column<int>(type: "integer", nullable: true),
                    binding_material_id = table.Column<int>(type: "integer", nullable: true),
                    saddle_material_id = table.Column<int>(type: "integer", nullable: true),
                    nut_material_id = table.Column<int>(type: "integer", nullable: true),
                    bridge_pin_material_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guitar_revisions", x => x.id);
                    table.ForeignKey(
                        name: "fk_guitar_revisions_brands_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brands",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_guitar_body_shapes_guitar_body_shape_id",
                        column: x => x.guitar_body_shape_id,
                        principalTable: "guitar_body_shapes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_guitar_bracing_types_guitar_bracing_type_id",
                        column: x => x.guitar_bracing_type_id,
                        principalTable: "guitar_bracing_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_guitar_types_guitar_type_id",
                        column: x => x.guitar_type_id,
                        principalTable: "guitar_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_guitars_guitar_id",
                        column: x => x.guitar_id,
                        principalTable: "guitars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_guitar_revisions_luthiers_luthier_id",
                        column: x => x.luthier_id,
                        principalTable: "luthiers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_constructions_back_material_const",
                        column: x => x.back_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_constructions_bridge_material_con",
                        column: x => x.bridge_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_constructions_fingerboard_materia",
                        column: x => x.fingerboard_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_constructions_neck_material_const",
                        column: x => x.neck_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_constructions_sides_material_cons",
                        column: x => x.sides_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_constructions_top_material_constr",
                        column: x => x.top_material_construction_id,
                        principalTable: "material_constructions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_grades_back_material_grade_id",
                        column: x => x.back_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_grades_bridge_material_grade_id",
                        column: x => x.bridge_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_grades_fingerboard_material_grade",
                        column: x => x.fingerboard_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_grades_neck_material_grade_id",
                        column: x => x.neck_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_grades_sides_material_grade_id",
                        column: x => x.sides_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_material_grades_top_material_grade_id",
                        column: x => x.top_material_grade_id,
                        principalTable: "material_grades",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_materials_back_material_id",
                        column: x => x.back_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_materials_binding_material_id",
                        column: x => x.binding_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_materials_bridge_material_id",
                        column: x => x.bridge_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_materials_bridge_pin_material_id",
                        column: x => x.bridge_pin_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_materials_fingerboard_material_id",
                        column: x => x.fingerboard_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_materials_neck_material_id",
                        column: x => x.neck_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_materials_nut_material_id",
                        column: x => x.nut_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_materials_saddle_material_id",
                        column: x => x.saddle_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_materials_sides_material_id",
                        column: x => x.sides_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_materials_top_material_id",
                        column: x => x.top_material_id,
                        principalTable: "materials",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_guitar_revisions_revision_metadatas_id",
                        column: x => x.id,
                        principalTable: "revision_metadatas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sound_profile_votes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guitar_id = table.Column<int>(type: "integer", nullable: false),
                    tone_bright_warm = table.Column<byte>(type: "smallint", nullable: false),
                    sustain_short_long = table.Column<byte>(type: "smallint", nullable: false),
                    attack_slow_fast = table.Column<byte>(type: "smallint", nullable: false),
                    clarity_muddy_clear = table.Column<byte>(type: "smallint", nullable: false),
                    dynamic_range_small_large = table.Column<byte>(type: "smallint", nullable: false),
                    body_thin_full = table.Column<byte>(type: "smallint", nullable: false),
                    volume_quiet_loud = table.Column<byte>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by_id = table.Column<int>(type: "integer", nullable: true),
                    updated_by_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sound_profile_votes", x => x.id);
                    table.ForeignKey(
                        name: "fk_sound_profile_votes_guitars_guitar_id",
                        column: x => x.guitar_id,
                        principalTable: "guitars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sound_profile_votes_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_sound_profile_votes_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                table: "AspNetRoleClaims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                table: "AspNetUserClaims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                table: "AspNetUserLogins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                table: "AspNetUserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_updated_by_id",
                table: "AspNetUsers",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_brands_country_id",
                table: "brands",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_back_material_construction_id",
                table: "guitar_revisions",
                column: "back_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_back_material_grade_id",
                table: "guitar_revisions",
                column: "back_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_back_material_id",
                table: "guitar_revisions",
                column: "back_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_binding_material_id",
                table: "guitar_revisions",
                column: "binding_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_brand_id",
                table: "guitar_revisions",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_bridge_material_construction_id",
                table: "guitar_revisions",
                column: "bridge_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_bridge_material_grade_id",
                table: "guitar_revisions",
                column: "bridge_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_bridge_material_id",
                table: "guitar_revisions",
                column: "bridge_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_bridge_pin_material_id",
                table: "guitar_revisions",
                column: "bridge_pin_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_country_id",
                table: "guitar_revisions",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_fingerboard_material_construction_id",
                table: "guitar_revisions",
                column: "fingerboard_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_fingerboard_material_grade_id",
                table: "guitar_revisions",
                column: "fingerboard_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_fingerboard_material_id",
                table: "guitar_revisions",
                column: "fingerboard_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_guitar_body_shape_id",
                table: "guitar_revisions",
                column: "guitar_body_shape_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_guitar_bracing_type_id",
                table: "guitar_revisions",
                column: "guitar_bracing_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_guitar_id",
                table: "guitar_revisions",
                column: "guitar_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_guitar_type_id",
                table: "guitar_revisions",
                column: "guitar_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_luthier_id",
                table: "guitar_revisions",
                column: "luthier_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_neck_material_construction_id",
                table: "guitar_revisions",
                column: "neck_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_neck_material_grade_id",
                table: "guitar_revisions",
                column: "neck_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_neck_material_id",
                table: "guitar_revisions",
                column: "neck_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_nut_material_id",
                table: "guitar_revisions",
                column: "nut_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_saddle_material_id",
                table: "guitar_revisions",
                column: "saddle_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_sides_material_construction_id",
                table: "guitar_revisions",
                column: "sides_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_sides_material_grade_id",
                table: "guitar_revisions",
                column: "sides_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_sides_material_id",
                table: "guitar_revisions",
                column: "sides_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_top_material_construction_id",
                table: "guitar_revisions",
                column: "top_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_top_material_grade_id",
                table: "guitar_revisions",
                column: "top_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitar_revisions_top_material_id",
                table: "guitar_revisions",
                column: "top_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_back_material_construction_id",
                table: "guitars",
                column: "back_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_back_material_grade_id",
                table: "guitars",
                column: "back_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_back_material_id",
                table: "guitars",
                column: "back_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_binding_material_id",
                table: "guitars",
                column: "binding_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_brand_id",
                table: "guitars",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_bridge_material_construction_id",
                table: "guitars",
                column: "bridge_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_bridge_material_grade_id",
                table: "guitars",
                column: "bridge_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_bridge_material_id",
                table: "guitars",
                column: "bridge_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_bridge_pin_material_id",
                table: "guitars",
                column: "bridge_pin_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_country_id",
                table: "guitars",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_fingerboard_material_construction_id",
                table: "guitars",
                column: "fingerboard_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_fingerboard_material_grade_id",
                table: "guitars",
                column: "fingerboard_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_fingerboard_material_id",
                table: "guitars",
                column: "fingerboard_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_guitar_body_shape_id",
                table: "guitars",
                column: "guitar_body_shape_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_guitar_bracing_type_id",
                table: "guitars",
                column: "guitar_bracing_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_guitar_type_id",
                table: "guitars",
                column: "guitar_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_luthier_id",
                table: "guitars",
                column: "luthier_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_name_brand_id",
                table: "guitars",
                columns: new[] { "name", "brand_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_guitars_neck_material_construction_id",
                table: "guitars",
                column: "neck_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_neck_material_grade_id",
                table: "guitars",
                column: "neck_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_neck_material_id",
                table: "guitars",
                column: "neck_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_nut_material_id",
                table: "guitars",
                column: "nut_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_saddle_material_id",
                table: "guitars",
                column: "saddle_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_sides_material_construction_id",
                table: "guitars",
                column: "sides_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_sides_material_grade_id",
                table: "guitars",
                column: "sides_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_sides_material_id",
                table: "guitars",
                column: "sides_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_top_material_construction_id",
                table: "guitars",
                column: "top_material_construction_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_top_material_grade_id",
                table: "guitars",
                column: "top_material_grade_id");

            migrationBuilder.CreateIndex(
                name: "ix_guitars_top_material_id",
                table: "guitars",
                column: "top_material_id");

            migrationBuilder.CreateIndex(
                name: "ix_luthier_revision_country_id",
                table: "luthier_revision",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_luthier_revision_luthier_id",
                table: "luthier_revision",
                column: "luthier_id");

            migrationBuilder.CreateIndex(
                name: "ix_luthiers_country_id",
                table: "luthiers",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_revision_metadatas_created_by_id",
                table: "revision_metadatas",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_revision_metadatas_parent_revision_id",
                table: "revision_metadatas",
                column: "parent_revision_id");

            migrationBuilder.CreateIndex(
                name: "ix_revision_metadatas_status",
                table: "revision_metadatas",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_revision_metadatas_updated_by_id",
                table: "revision_metadatas",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_revision_metadatas_version",
                table: "revision_metadatas",
                column: "version");

            migrationBuilder.CreateIndex(
                name: "ix_sound_profile_votes_created_by_id",
                table: "sound_profile_votes",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_sound_profile_votes_guitar_id",
                table: "sound_profile_votes",
                column: "guitar_id");

            migrationBuilder.CreateIndex(
                name: "ix_sound_profile_votes_updated_by_id",
                table: "sound_profile_votes",
                column: "updated_by_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "guitar_revisions");

            migrationBuilder.DropTable(
                name: "luthier_revision");

            migrationBuilder.DropTable(
                name: "sound_profile_votes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "revision_metadatas");

            migrationBuilder.DropTable(
                name: "guitars");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "guitar_body_shapes");

            migrationBuilder.DropTable(
                name: "guitar_bracing_types");

            migrationBuilder.DropTable(
                name: "guitar_types");

            migrationBuilder.DropTable(
                name: "luthiers");

            migrationBuilder.DropTable(
                name: "material_constructions");

            migrationBuilder.DropTable(
                name: "material_grades");

            migrationBuilder.DropTable(
                name: "materials");

            migrationBuilder.DropTable(
                name: "countries");
        }
    }
}
