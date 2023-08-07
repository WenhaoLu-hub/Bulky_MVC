﻿// <auto-generated />
using BulkyBook.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BulkyBook.Models.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CategoryId"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryId = 1,
                            DisplayOrder = 1,
                            Name = "Action"
                        },
                        new
                        {
                            CategoryId = 2,
                            DisplayOrder = 2,
                            Name = "Novel"
                        },
                        new
                        {
                            CategoryId = 3,
                            DisplayOrder = 3,
                            Name = "Science Fiction"
                        });
                });

            modelBuilder.Entity("BulkyBook.Models.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ProductId"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("ListPrice")
                        .HasColumnType("double precision");

                    b.Property<double>("ListPrice100")
                        .HasColumnType("double precision");

                    b.Property<double>("ListPrice50")
                        .HasColumnType("double precision");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            Author = "William Somerset Maugham",
                            CategoryId = 1,
                            Description = "The Moon and Sixpence is a novel by W. Somerset Maugham, first published on 15 April 1919. It is told in episodic form by a first-person narrator providing a series of glimpses into the mind and soul of the central character, Charles Strickland, a middle-aged English stockbroker, who abandons his wife and children abruptly to pursue his desire to become an artist. The story is, in part, based on the life of the painter Paul Gauguin.",
                            ISBN = "9781598185218",
                            ImageUrl = "",
                            ListPrice = 5.9900000000000002,
                            ListPrice100 = 4.9900000000000002,
                            ListPrice50 = 5.29,
                            Title = "The Moon and SixPence"
                        },
                        new
                        {
                            ProductId = 2,
                            Author = "Ernest Hemingway",
                            CategoryId = 1,
                            Description = "The Old Man and the Sea is a novella written by the American author Ernest Hemingway in 1951 in Cayo Blanco (Cuba), and published in 1952.[1] It was the last major work of fiction written by Hemingway that was published during his lifetime. One of his most famous works, it tells the story of Santiago, an aging Cuban fisherman who struggles with a giant marlin far out in the Gulf Stream off the coast of Cuba.",
                            ISBN = "9780684830490",
                            ImageUrl = "",
                            ListPrice = 5.9900000000000002,
                            ListPrice100 = 4.9900000000000002,
                            ListPrice50 = 5.29,
                            Title = "The Old Man and the Sea"
                        },
                        new
                        {
                            ProductId = 3,
                            Author = "F. Scott Fitzgerald",
                            CategoryId = 1,
                            Description = "The Great Gatsby is a 1925 novel by American writer F. Scott Fitzgerald. Set in the Jazz Age on Long Island, near New York City, the novel depicts first-person narrator Nick Carraway's interactions with mysterious millionaire Jay Gatsby and Gatsby's obsession to reunite with his former lover, Daisy Buchanan.",
                            ISBN = "0743273567",
                            ImageUrl = "",
                            ListPrice = 5.9900000000000002,
                            ListPrice100 = 4.9900000000000002,
                            ListPrice50 = 5.29,
                            Title = "The Great Gatsby"
                        });
                });

            modelBuilder.Entity("BulkyBook.Models.Models.Product", b =>
                {
                    b.HasOne("BulkyBook.Models.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}
