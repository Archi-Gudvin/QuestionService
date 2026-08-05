namespace QuestionService.Tests.FunctionalTests.Helpers;

internal static class GraphQlHelper
{
    public const string GraphQlEndpoint = "/graphql";

    public const string RequestAllQuery = """
                                          {
                                            questions(after: "e30x", first: 2, order: [{ id: ASC }]) {
                                              edges {
                                                cursor
                                                node {
                                                  id
                                                  title
                                                  body
                                                  reputation
                                                  viewCount
                                                  userId
                                                  createdAt
                                                  lastModifiedAt
                                                  tags {
                                                    id
                                                    name
                                                    description
                                                    questions {
                                                      id
                                                      title
                                                    }
                                                  }
                                                  votes {
                                                    userId
                                                    voteTypeId
                                                    voteType {
                                                      id
                                                      name
                                                      minReputationToVote
                                                      reputationChange
                                                      votes {
                                                        userId
                                                      }
                                                    }
                                                    question {
                                                      id
                                                      title
                                                    }
                                                  }
                                                  views {
                                                    id
                                                    questionId
                                                    userId
                                                    userIp
                                                    userFingerprint
                                                    question {
                                                      id
                                                      title
                                                    }
                                                  }
                                                }
                                              }
                                              pageInfo {
                                                endCursor
                                                startCursor
                                                hasNextPage
                                                hasPreviousPage
                                              }
                                              totalCount
                                            }

                                            questionVotes(skip: 1, take: 2) {
                                              items {
                                                userId
                                                voteTypeId
                                                voteType {
                                                  id
                                                  name
                                                  minReputationToVote
                                                  reputationChange
                                                  votes {
                                                    userId
                                                  }
                                                }
                                                question {
                                                  id
                                                  title
                                                }
                                              }
                                              totalCount
                                            }

                                            tags(after: "e30x", first: 2, order: [{ id: ASC }]) {
                                              edges {
                                                cursor
                                                node {
                                                  id
                                                  name
                                                  description
                                                  questions {
                                                    id
                                                    title
                                                  }
                                                }
                                              }
                                              pageInfo {
                                                endCursor
                                                startCursor
                                                hasNextPage
                                                hasPreviousPage
                                              }
                                              totalCount
                                            }

                                            questionViews(skip: 1, take: 2, order: [{ question: { createdAt: DESC } }]) {
                                              items {
                                                id
                                                questionId
                                                userId
                                                userIp
                                                userFingerprint
                                                question {
                                                  id
                                                  title
                                                }
                                              }
                                              totalCount
                                            }

                                            questionVoteTypes(skip: 1, take: 2, order: [{ reputationChange: ASC }]) {
                                              items {
                                                id
                                                name
                                                minReputationToVote
                                                reputationChange
                                                votes {
                                                  userId
                                                  question {
                                                    title
                                                  }
                                                }
                                              }
                                              totalCount
                                            }
                                          }
                                          """;

    public const string RequestQuestionsWithTagsQuery = """
                                                        {
                                                          questions(after: "e30x", first: 2, order: [{ id: ASC }]) {
                                                            edges {
                                                              cursor
                                                              node {
                                                                id
                                                                title
                                                                body
                                                                userId
                                                                reputation
                                                                viewCount
                                                                createdAt
                                                                lastModifiedAt
                                                                tags {
                                                                  name
                                                                }
                                                              }
                                                            }
                                                            pageInfo {
                                                              endCursor
                                                              startCursor
                                                              hasNextPage
                                                              hasPreviousPage
                                                            }
                                                            totalCount
                                                          }
                                                        }
                                                        """;

    public const string RequestWithWrongArgument = """
                                                   tag(wrongArg) {
                                                     name
                                                     description
                                                     questions {
                                                       id
                                                       title
                                                     }
                                                   }
                                                   """;

    public const string RequestWithInvalidPaginationQuery = """
                                                            {
                                                              questions(after: "notValidAfter", first: -1) {
                                                                edges {
                                                                  cursor
                                                                  node {
                                                                    id
                                                                    title
                                                                    body
                                                                    userId
                                                                    reputation
                                                                    viewCount
                                                                    createdAt
                                                                    lastModifiedAt
                                                                    tags {
                                                                      id
                                                                      name
                                                                      description
                                                                      questions {
                                                                        id
                                                                        title
                                                                      }
                                                                    }
                                                                    votes {
                                                                      userId
                                                                      voteTypeId
                                                                      voteType {
                                                                       id
                                                                       name
                                                                       reputationChange
                                                                       minReputationToVote
                                                                      }
                                                                      question {
                                                                        id
                                                                        title
                                                                      }
                                                                    }
                                                                    views {
                                                                      id
                                                                      questionId
                                                                      userId
                                                                      userIp
                                                                      userFingerprint
                                                                      question {
                                                                        id
                                                                        title
                                                                      }
                                                                    }
                                                                  }
                                                                }
                                                                pageInfo {
                                                                  endCursor
                                                                  startCursor
                                                                  hasNextPage
                                                                  hasPreviousPage
                                                                }
                                                                totalCount
                                                              }
                                                              questionVotes(skip: -1, take: 101) {
                                                                items {
                                                                  userId
                                                                  voteTypeId
                                                                  voteType {
                                                                   id
                                                                   name
                                                                   reputationChange
                                                                   minReputationToVote
                                                                  }
                                                                  question {
                                                                    id
                                                                    title
                                                                  }
                                                                }
                                                                totalCount
                                                              }
                                                              tags(after: "notValidAftre", last: 2, order: []) {
                                                                edges {
                                                                  cursor
                                                                  node {
                                                                    id
                                                                    name
                                                                    description
                                                                    questions {
                                                                      id
                                                                      title
                                                                    }
                                                                  }
                                                                }
                                                                pageInfo {
                                                                  endCursor
                                                                  startCursor
                                                                  hasNextPage
                                                                  hasPreviousPage
                                                                }
                                                                totalCount
                                                              }
                                                              questionViews(skip: -1, take: -1) {
                                                                items {
                                                                  id
                                                                  questionId
                                                                  userId
                                                                  userIp
                                                                  userFingerprint
                                                                  question {
                                                                    id
                                                                    title
                                                                  }
                                                                }
                                                                totalCount
                                                              }
                                                            }
                                                            """;

    public static string RequestAllByIdsQuery(long questionId, long voteQuestionId, long voteUserId,
      long tagId, long viewId, long voteTypeId)
    {
        return """
               {
                 question(id: $QUESTIONID) {
                   id
                   title
                   body
                   userId
                   reputation
                   viewCount
                   createdAt
                   lastModifiedAt
                   tags {
                     id
                     name
                     description
                     questions {
                       id
                       title
                     }
                   }
                   votes {
                     userId
                     voteTypeId
                     voteType {
                       id
                       name
                       reputationChange
                       minReputationToVote
                       votes {
                         userId
                       }
                     }
                     question {
                       id
                       title
                     }
                   }
                   views {
                     id
                     questionId
                     userId
                     userIp
                     userFingerprint
                     question {
                       id
                       title
                     }
                   }
                 }
                 questionVote(userId: $VOTEUSERID, questionId: $VOTEQUESTIONID) {
                   userId
                   voteTypeId
                   voteType {
                     id
                     name
                     reputationChange
                     minReputationToVote
                     votes {
                       userId
                     }
                   }
                   question {
                     id
                     title
                   }
                 }
                 tag(id: $TAGID) {
                   name
                   description
                   questions {
                     id
                     title
                   }
                 }
                 questionView(id: $VIEWID) {
                   id
                   questionId
                   userId
                   userIp
                   userFingerprint
                   question {
                     id
                     title
                   }
                 }
                 questionVoteType(id: $VOTETYPEID) { 
                   id
                   name
                   minReputationToVote
                   reputationChange
                   votes{
                     userId
                     question{
                       title
                     }
                   }
                 }
               }
               """
            .Replace("$QUESTIONID", questionId.ToString())
            .Replace("$VOTEQUESTIONID", voteQuestionId.ToString())
            .Replace("$VOTEUSERID", voteUserId.ToString())
            .Replace("$TAGID", tagId.ToString())
            .Replace("$VIEWID", viewId.ToString())
            .Replace("$VOTETYPEID", voteTypeId.ToString());
    }

    public static string RequestQuestionByIdQuery(long questionId)
    {
        return """
               {
                 question(id: $QUESTIONID) {
                   id
                   title
                   body
                   userId
                   reputation
                   viewCount
                   createdAt
                   lastModifiedAt
                   tags {
                     id
                     name
                     description
                     questions {
                       id
                       title
                     }
                   }
                 }  
               }
               """
            .Replace("$QUESTIONID", questionId.ToString());
    }
}